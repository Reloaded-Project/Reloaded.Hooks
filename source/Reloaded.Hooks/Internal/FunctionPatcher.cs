using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Buffers;
using Reloaded.Memory.Buffers.Internal.Kernel32;
using Reloaded.Memory.Sources;
using SharpDisasm;
using SharpDisasm.Udis86;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Disassembles provided bytes and tries to detect other function hooks.
    /// Generates <see cref="Internal.Patch"/>es which fix other software's hooks to co-operate with ours.
    /// </summary>
    public class FunctionPatcher
    {
        private ArchitectureMode _architecture;
        private FunctionHookOptions _options;
        private ProcessModule[] _modules;

        public FunctionPatcher(bool is64Bit, FunctionHookOptions options = null)
        {
            _architecture = is64Bit ? ArchitectureMode.x86_64
                                    : ArchitectureMode.x86_32;
            _options = options ?? new FunctionHookOptions();
        }

        public FunctionPatcher(ArchitectureMode mode, FunctionHookOptions options = null)
        {
            _architecture = mode;
            _options = options ?? new FunctionHookOptions();
        }

        /// <summary>
        /// Rewrites existing functions (supplied as list of bytes) converting relative to absolute jumps as well
        /// as the return addresses from given jumps.
        /// See Source Code for more details.
        /// </summary>
        /// <param name="oldFunction">The function to rewrite.</param>
        /// <param name="baseAddress">The original address of the function.</param>
        /// <returns></returns>
        public FunctionPatch Patch(List<byte> oldFunction, IntPtr baseAddress)
        {
            /*
                === What this is ===    

                This function attempts to find and patch other non-Reloaded API hooks.

                It does this by rewriting a given function `oldfunction` converting relative to absolute jumps.
                In addition, it follows the target of all of the original relative jumps, and tries to detect
                and correct existing jumps back to game code to a new address.

                The purpose is to allow to stack Reloaded hooks on non-Reloaded hooks; 
                e.g. Steam hooks DirectX EndScene; we want to hook the Steam hooked scene.         
            */

            /*
                === Cases Handled ===

                Where '*' indicates 0 or more.

                This function patches:
                    1. Relative immediate jumps.        
                        nop*
                        jmp 0x123456
                        nop*
                    
                    2. Push + Return
                        nop*
                        push 0x612403
                        ret
                        nop*

                    3. RIP Relative Addressing (X64)
                        nop*
                        JMP [RIP+0]
                        nop*

                This function ignores:
                    Indirect memory operand pointer jumps.        
                        jmp [0x123456]
             */

            FunctionPatch functionPatch = new FunctionPatch();
            long reloadedHookEndAddress = oldFunction.Count + (long)baseAddress; // End of our own hook.

            Disassembler  disassembler = new Disassembler(oldFunction.ToArray(), _architecture, (ulong)baseAddress, true);
            Instruction[] instructions = disassembler.Disassemble().ToArray();
            
            for (int x = 0; x < instructions.Length; x++)
            {
                Instruction instruction     = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Length) ? instructions[x + 1] : null;
                JumpDetails jumpDetails;

                if      (IsRelativeJump(instruction) && !IsJumpTargetInAModule((long) instruction.PC, GetRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRelativeJump(instruction, functionPatch);
                else if (IsRIPRelativeJump(instruction) && !IsJumpTargetInAModule((long)instruction.PC, (long)GetRewriteRIPRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRIPRelativeJump(instruction, functionPatch);
                else if (nextInstruction != null && IsPushReturn(instruction, nextInstruction) && !IsJumpTargetInAModule((long)instruction.PC, GetPushReturnTarget(instruction)) )
                    jumpDetails = RewritePushReturn(instruction, nextInstruction, functionPatch);
                else
                {
                    functionPatch.NewFunction.AddRange(instruction.Bytes);
                    continue; // No patching on no addresses to patch.
                }

                PatchReturnAddresses(jumpDetails, functionPatch, reloadedHookEndAddress);
            }

            return functionPatch;
        }

        /// <summary>
        /// Patches all jumps pointing to originalJmpTarget to point to newJmpTarget.
        /// </summary>
        /// <param name="searchRange">Range of addresses where to patch jumps.</param>
        /// <param name="originalJmpTarget">Address range of JMP targets to patch with newJmpTarget.</param>
        /// <param name="newJmpTarget">The new address instructions should jmp to.</param>
        private List<Patch> PatchJumpTargets_Internal(AddressRange searchRange, AddressRange originalJmpTarget, long newJmpTarget)
        {
            var patches = new List<Patch>();
            int length = (int)(searchRange.EndPointer - searchRange.StartPointer);
            var memory = TryReadFromMemory((IntPtr) searchRange.StartPointer, length);

            Disassembler disassembler = new Disassembler(memory, _architecture, (ulong)searchRange.StartPointer, true);
            Instruction[] instructions = disassembler.Disassemble().ToArray();

            for (int x = 0; x < instructions.Length; x++)
            {
                Instruction instruction = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Length) ? instructions[x + 1] : null;

                if (IsRelativeJump(instruction))
                    PatchRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                else if (IsRIPRelativeJump(instruction))
                    PatchRIPRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                else if (nextInstruction != null && IsPushReturn(instruction, nextInstruction))
                    PatchPushReturn(instruction, ref originalJmpTarget, newJmpTarget, patches);
            }

            // Return all the addresses to patch!.
            return patches;
        }

        /* == Rewrite Functions ==
         
          These functions simply covert a specific jump type such as Relative Immediate Op jmp
          `jmp 0x123456` to absolute jumps.

          ... and add the results to patch.NewFunction
        */

        private long GetPushReturnTarget(Instruction pushInstruction) => GetOperandOffset(pushInstruction.Operands[0]);
        private long GetRelativeJumpTarget(Instruction instruction) => (long)instruction.PC + GetOperandOffset(instruction.Operands[0]);
        private IntPtr GetRewriteRIPRelativeJumpTarget(Instruction instruction)
        {
            IntPtr pointerAddress = (IntPtr)((long)instruction.PC + GetOperandOffset(instruction.Operands[0]));
            CurrentProcess.Read(pointerAddress, out IntPtr targetAddress);
            return targetAddress;
        }

        private JumpDetails RewriteRelativeJump(Instruction instruction, FunctionPatch patch)
        {
            long originalJmpTarget = GetRelativeJumpTarget(instruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump((IntPtr) originalJmpTarget, Is64Bit()));
            return new JumpDetails((long) instruction.PC, originalJmpTarget);
        }

        private JumpDetails RewriteRIPRelativeJump(Instruction instruction, FunctionPatch patch)
        {
            IntPtr targetAddress = GetRewriteRIPRelativeJumpTarget(instruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump(targetAddress, Is64Bit()));
            return new JumpDetails((long) instruction.PC, (long) targetAddress);
        }

        private JumpDetails RewritePushReturn(Instruction pushInstruction, Instruction retInstruction, FunctionPatch patch)
        {
            // Push does not support 64bit immediates. This makes our life considerably easier.
            long originalJmpTarget = GetPushReturnTarget(pushInstruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump((IntPtr)originalJmpTarget, Is64Bit()));
            return new JumpDetails((long) retInstruction.PC, originalJmpTarget);
        }

        /* == Patch Function ==
         
          First a function stub is generated containing:
          1. Opcodes between Instruction.PC and "newAddress"
          2. a jmp to "newAddress".

          Then these functions look at the original JMP target and look for jumps back to 
          the end of the instruction.          
          
          Patches are then generated that convert those jumps back to jumps to the location of
          function stub.

          Patches are added to patch.Patches.
        */

        private void PatchReturnAddresses(JumpDetails jumpDetails, FunctionPatch patch, long newAddress)
        {
            long originalJmpTarget    = jumpDetails.JumpOpcodeTarget;
            long initialSearchPointer = originalJmpTarget;
            GetSearchRange(ref initialSearchPointer, out long searchLength);

            /* Get original opcodes after original JMP instruction. */

            IntPtr startRemainingOpcodes = (IntPtr)jumpDetails.JumpOpcodeEnd;
            int lengthRemainingOpcodes   = (int)(newAddress - (long)startRemainingOpcodes);
            var remainingInstructions = TryReadFromMemory(startRemainingOpcodes, lengthRemainingOpcodes);

            /* Build function stub + patches. */

            // Must guarantee relative jumps to be patches can reach our new prologue
            // as such must get range of search first before creating stub.
            long maxDisplacement         = Int32.MaxValue - searchLength;
            IntPtr newOriginalPrologue   = Utilities.InsertJump(remainingInstructions, Is64Bit(), newAddress, originalJmpTarget, maxDisplacement);
            
            // Catch all return addresses in page range.
            var pageRange       = new AddressRange(initialSearchPointer, initialSearchPointer + searchLength);
            var jumpTargetRange = new AddressRange((long) startRemainingOpcodes, newAddress);

            patch.Patches = PatchJumpTargets(pageRange, originalJmpTarget, jumpTargetRange, newOriginalPrologue);
        }

        internal List<Patch> PatchJumpTargets(AddressRange searchRange, long originalJmpTarget, AddressRange jumpTargetRange, IntPtr newOriginalPrologue)
        {
            /*
                On both modern Intel and AMD CPUs, the instruction decoder fetches instructions 16 bytes per cycle.
                These 16 bytes are always aligned, so you can only fetch 16 bytes from a multiple of 16.

                Some hooks, such as Reloaded.Hooks itself exploit this for micro-optimisation.
                Valve seems to be doing this with the Steam overlay too.
            */
            const int intelCodeAlignment = 16;
            const int immediateAreaSize = intelCodeAlignment * 4; // Keep as multiple of code alignment.
            var result = new List<Patch>();

            /*
                When looking at a whole page range, there are occasional cases where the 
                padding (e.g. with 00 padding) may lead to having the instruction incorrectly decoded 
                if we start disassembling from the page.

                Therefore we first test only the immediate area at specifically crafted starting points
                to take account either the code being aligned or unaligned.

                This should fix e.g. the odd case of Steam Overlay hooks not being patched.
                We only expect one jump in practically all cases so it's safe to end if a single jump is found.
            */

            // Case 1: Our jump target is part of an aligned 16 byte code frame.
            // Check 1 frame before + 3 frames ahead
            if (TryCodeAlignmentRange(AddressRange.FromStartAndLength(originalJmpTarget / intelCodeAlignment * intelCodeAlignment, immediateAreaSize)))
                return result;

            // Case 2: Original code comes before jump target and is in the frame before.
            // Check 2 frame before + 0 frames ahead
            if (TryCodeAlignmentRange(AddressRange.FromStartAndLength((originalJmpTarget / intelCodeAlignment * intelCodeAlignment) - intelCodeAlignment, intelCodeAlignment * 2)))
                return result;

            // Case 3: Code is unaligned and original code comes right after the jump target.
            // 0 frames before + 2 frames ahead.
            if (TryCodeAlignmentRange(AddressRange.FromStartAndLength(originalJmpTarget, intelCodeAlignment * 2)))
                return result;

            // Case 4: Fall back to searching whole memory page.
            // This is successful 90% of the time, but sometimes fails due to code misalignment
            // (disassembler can interpret 00 as beginning of an instruction when it's padding).
            var patchesForPage = PatchJumpTargets_Internal(searchRange, jumpTargetRange, (long) newOriginalPrologue);
            result.AddRange(patchesForPage);
            return result;

            bool TryCodeAlignmentRange(AddressRange range)
            {
                // Clamp to current memory page if the start/end cannot be read.
                if (range.StartPointer < searchRange.StartPointer && Utilities.IsBadReadPtr((IntPtr) range.StartPointer))
                    range.StartPointer = searchRange.StartPointer;

                if (range.EndPointer > searchRange.EndPointer && Utilities.IsBadReadPtr((IntPtr) range.EndPointer))
                    range.EndPointer = searchRange.EndPointer;

                var patchesForImmediateArea = PatchJumpTargets_Internal(range, jumpTargetRange, (long) newOriginalPrologue);
                result.AddRange(patchesForImmediateArea);
                return patchesForImmediateArea.Count > 0;
            }
        }

        /// <summary>
        /// Creates patch for a relative jump, if necessary.
        /// </summary>
        private void PatchRelativeJump(Instruction instruction, ref AddressRange originalJmpTarget, long newJmpTarget, List<Patch> patches)
        {
            long jumpTargetAddress = (long)instruction.PC + GetOperandOffset(instruction.Operands[0]);
            if (originalJmpTarget.Contains(jumpTargetAddress))
            {
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump((IntPtr) instruction.Offset, (IntPtr) newJmpTarget, Is64Bit());
                patches.Add(new Patch((IntPtr) instruction.Offset, relativeJumpBytes));
            }
        }

        /// <summary>
        /// Creates patch for a RIP relative jump, if necessary.
        /// </summary>
        private void PatchRIPRelativeJump(Instruction instruction, ref AddressRange originalJmpTarget, long newJmpTarget, List<Patch> patches)
        {
            IntPtr pointerAddress = (IntPtr)((long)instruction.PC + GetOperandOffset(instruction.Operands[0]));
            CurrentProcess.Read(pointerAddress, out IntPtr jumpTargetAddress);

            if (originalJmpTarget.Contains((long) jumpTargetAddress))
            {
                // newJmpTarget is guaranteed to be in range.
                // Relative jump uses less bytes, so using it is also safe.
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump((IntPtr) instruction.Offset, (IntPtr) newJmpTarget, Is64Bit());
                patches.Add(new Patch((IntPtr)instruction.Offset, relativeJumpBytes));
            }
        }

        /// <summary>
        /// Creates patch for a push + return combo, if necessary.
        /// </summary>
        private void PatchPushReturn(Instruction instruction, ref AddressRange originalJmpTarget, long newJmpTarget, List<Patch> patches)
        {
            long jumpTargetAddress = GetOperandOffset(instruction.Operands[0]);

            if (originalJmpTarget.Contains((long)jumpTargetAddress))
            {
                // Push + Return & JMP Absolute use the same number of bytes in X86. but not in X64.
                // We must create a new Push + Return to an absolute jump.
                byte[] absoluteJump = Utilities.AssembleAbsoluteJump((IntPtr) newJmpTarget, Is64Bit());
                var buffer = Utilities.FindOrCreateBufferInRange(absoluteJump.Length);
                var absoluteJmpPointer = buffer.Add(absoluteJump);

                byte[] newPushReturn = Utilities.AssemblePushReturn(absoluteJmpPointer, Is64Bit());
                patches.Add(new Patch((IntPtr)instruction.Offset, newPushReturn));
            }
        }

        /// <summary>
        /// [Part of PatchJumpTargets_Internal]
        /// Obtains the address range to perform search for jumps back by modifying a given searchPointer and giving a searchRange.
        /// </summary>
        /// <param name="searchPointer">The initial pointer from which to deduce the search range.</param>
        /// <param name="searchLength"> The length of the search.</param>
        internal void GetSearchRange(ref long searchPointer, out long searchLength)
        {
            searchLength = 0;

            // Search in modules (if necessary).
            if (_options.SearchInModules)
            {
                // Cache the module list.
                foreach (ProcessModule module in GetCachedModules())
                {
                    long minimumAddress = (long)module.BaseAddress;
                    long maximumAddress = (long)module.BaseAddress + module.ModuleMemorySize;

                    if (searchPointer >= minimumAddress && searchPointer <= maximumAddress)
                    {
                        searchPointer = minimumAddress;
                        searchLength = module.ModuleMemorySize;
                    }
                }
            }

            // If the search range is 0 (our address is not in a module),
            // consider instead scanning the whole memory page.
            if (searchLength == 0)
            {
                searchLength = Environment.SystemPageSize;
                searchPointer -= searchPointer % searchLength;
            }
        }

        /// <summary>
        /// Attempts to read out a number of bytes from unmanaged memory.
        /// </summary>
        /// <param name="address">Address to read from.</param>
        /// <param name="size">The size of memory.</param>
        private byte[] TryReadFromMemory(IntPtr address, int size)
        {
            byte[] memory;
            try
            {
                CurrentProcess.ReadRaw(address, out memory, size);
            }
            catch (Exception)
            {
                /* Ignore exception, and only take 2nd one. */
                CurrentProcess.SafeReadRaw(address, out memory, size);
            }

            return memory;
        }

        /* Condition check methods for class. */

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRelativeJump(Instruction instruction)
        {
            return instruction.Mnemonic == ud_mnemonic_code.UD_Ijmp && 
                   instruction.Operands.Length > 0 && 
                   instruction.Operands[0].Type == ud_type.UD_OP_JIMM;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPushReturn(Instruction instruction, Instruction nextInstruction)
        {
            return instruction.Mnemonic == ud_mnemonic_code.UD_Ipush &&
                   instruction.Operands.Length >= 1 &&
                   instruction.Operands[0].Size == 32 && // Does not support 64bit immediates.
                   nextInstruction.Mnemonic == ud_mnemonic_code.UD_Iret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool Is64Bit()
        {
            return _architecture == ArchitectureMode.x86_64;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRIPRelativeJump(Instruction instruction)
        {
            return instruction.Mnemonic == ud_mnemonic_code.UD_Ijmp &&
                   _architecture == ArchitectureMode.x86_64 &&
                   instruction.Operands.Length >= 1 &&
                   instruction.Operands[0].Base == ud_type.UD_R_RIP;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsJumpTargetInAModule(long source, long target)
        {
            if (!_options.VerifyJumpTargetsModule)
                return false;

            foreach (ProcessModule module in GetCachedModules())
            {
                var range = new AddressRange((long) module.BaseAddress, (long) (module.BaseAddress + module.ModuleMemorySize));
                if (range.Contains(source) && range.Contains(target))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Grabs a cached copy of the module list.
        /// </summary>
        private ProcessModule[] GetCachedModules()
        {
            if (_modules != null)
                return _modules;

            var modules = Process.GetCurrentProcess().Modules;
            _modules = new ProcessModule[modules.Count];
            modules.CopyTo(_modules, 0);
            return _modules;
        }

        /* Other Utility Functions */

        /// <summary>
        /// Obtains the offset of a relative immediate operand, else throws exception,
        /// </summary>
        private long GetOperandOffset(Operand operand)
        {
            switch (operand.Size)
            {
                case 8:
                    return operand.LvalSByte;
                case 16:
                    return operand.LvalSWord;
                case 32:
                    return operand.LvalSDWord;
                case 64:
                    return operand.LvalSQWord;
                default:
                    throw new Exception("Unknown operand size");
            }
        }

        private struct JumpDetails
        {
            /// <summary>
            /// Pointer to end of the opcode combination that causes the jump.
            /// </summary>
            public long JumpOpcodeEnd;

            /// <summary>
            /// Where the opcode jumps to.
            /// </summary>
            public long JumpOpcodeTarget;

            public JumpDetails(long jumpOpcodeEnd, long jumpOpcodeTarget)
            {
                JumpOpcodeEnd = jumpOpcodeEnd;
                JumpOpcodeTarget = jumpOpcodeTarget;
            }
        }
    }
}
