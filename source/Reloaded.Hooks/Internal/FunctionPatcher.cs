using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Iced.Intel;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Sources;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Disassembles provided bytes and tries to detect other function hooks.
    /// Generates <see cref="Internal.Patch"/>es which fix other software's hooks to co-operate with ours.
    /// </summary>
    public class FunctionPatcher
    {
        private bool _is64Bit;
        private FunctionHookOptions _options;
        private ProcessModule[] _modules;

        public FunctionPatcher(bool is64Bit, FunctionHookOptions options = null)
        {
            _is64Bit = is64Bit;
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
        public unsafe FunctionPatch Patch(List<byte> oldFunction, nuint baseAddress)
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

            FunctionPatch functionPatch  = new FunctionPatch();
            nuint reloadedHookEndAddress = baseAddress + (nuint)oldFunction.Count; // End of our own hook.

            var oldFunctionArr = oldFunction.ToArray();
            var decoder = Decoder.Create(_is64Bit ? 64 : 32, new ByteArrayCodeReader(oldFunctionArr));
            decoder.IP = baseAddress;
            var endRip  = (nuint)oldFunction.Count + baseAddress;

            var instructions = new List<Instruction>();
            while (decoder.IP < endRip)
                instructions.Add(decoder.Decode());

            for (int x = 0; x < instructions.Count; x++)
            {
                Instruction instruction     = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Count) ? instructions[x + 1] : default;
                JumpDetails jumpDetails;

                if      (IsRelativeJump(instruction) && !IsJumpTargetInAModule(PC(instruction), GetRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRelativeJump(instruction, functionPatch);
                else if (IsRIPRelativeJump(instruction) && !IsJumpTargetInAModule(PC(instruction), (nuint)GetRewriteRIPRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRIPRelativeJump(instruction, functionPatch);
                else if (nextInstruction != default && IsPushReturn(instruction, nextInstruction) && !IsJumpTargetInAModule(PC(instruction), GetPushReturnTarget(instruction)) )
                    jumpDetails = RewritePushReturn(instruction, nextInstruction, functionPatch);
                else
                {
                    var offset  = instruction.IP - baseAddress;
                    var oldInsn = oldFunctionArr.AsSpan().Slice((int)offset, instruction.Length);
                    functionPatch.NewFunction.AddRange(oldInsn.ToArray());
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
        private List<Patch> PatchJumpTargets_Internal(AddressRange searchRange, AddressRange originalJmpTarget, nuint newJmpTarget)
        {
            var patches = new List<Patch>();
            int length = (int)(searchRange.EndPointer - searchRange.StartPointer);
            var memory = TryReadFromMemory(searchRange.StartPointer, length);
            
            var decoder = Decoder.Create(_is64Bit ? 64 : 32, new ByteArrayCodeReader(memory));
            decoder.IP = searchRange.StartPointer;
            var endRip = (nuint)memory.Length + searchRange.StartPointer;

            var instructions = new List<Instruction>();
            while (decoder.IP < endRip)
                instructions.Add(decoder.Decode());

            for (int x = 0; x < instructions.Count; x++)
            {
                Instruction instruction = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Count) ? instructions[x + 1] : default;

                if (IsRelativeJump(instruction))
                    PatchRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                else if (IsRIPRelativeJump(instruction))
                    PatchRIPRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                else if (nextInstruction != default && IsPushReturn(instruction, nextInstruction))
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

        private nuint GetPushReturnTarget(Instruction pushInstruction) => (nuint)GetOperandOffset(pushInstruction);

        private nuint GetRelativeJumpTarget(Instruction instruction) => (nuint)GetJumpTarget(instruction);

        private nuint GetRewriteRIPRelativeJumpTarget(Instruction instruction)
        {
            var pointerAddress = instruction.IPRelativeMemoryAddress;
            CurrentProcess.Read((nuint)pointerAddress, out nuint targetAddress);
            return targetAddress;
        }

        private JumpDetails RewriteRelativeJump(Instruction instruction, FunctionPatch patch)
        {
            nuint originalJmpTarget = GetRelativeJumpTarget(instruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump(originalJmpTarget, _is64Bit));
            return new JumpDetails(PC(instruction), originalJmpTarget);
        }

        private JumpDetails RewriteRIPRelativeJump(Instruction instruction, FunctionPatch patch)
        {
            nuint targetAddress = GetRewriteRIPRelativeJumpTarget(instruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump(targetAddress, _is64Bit));
            return new JumpDetails(PC(instruction), targetAddress);
        }

        private JumpDetails RewritePushReturn(Instruction pushInstruction, Instruction retInstruction, FunctionPatch patch)
        {
            // Push does not support 64bit immediates. This makes our life considerably easier.
            nuint originalJmpTarget = GetPushReturnTarget(pushInstruction);
            patch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump(originalJmpTarget, _is64Bit));
            return new JumpDetails(PC(retInstruction), originalJmpTarget);
        }

        /* == Patch Function ==
         
          First a function stub is generated containing:
          1. Opcodes between Instruction.IP and "newAddress"
          2. a jmp to "newAddress".

          Then these functions look at the original JMP target and look for jumps back to 
          the end of the instruction.          
          
          Patches are then generated that convert those jumps back to jumps to the location of
          function stub.

          Patches are added to patch.Patches.
        */

        private void PatchReturnAddresses(JumpDetails jumpDetails, FunctionPatch patch, nuint newAddress)
        {
            nuint originalJmpTarget    = jumpDetails.JumpOpcodeTarget;
            nuint initialSearchPointer = originalJmpTarget;
            GetSearchRange(ref initialSearchPointer, out nuint searchLength);

            /* Get original opcodes after original JMP instruction. */

            nuint startRemainingOpcodes = jumpDetails.JumpOpcodeEnd;
            int lengthRemainingOpcodes   = (int)(newAddress - startRemainingOpcodes);
            var remainingInstructions = TryReadFromMemory(startRemainingOpcodes, lengthRemainingOpcodes);

            /* Build function stub + patches. */

            // Must guarantee relative jumps to be patches can reach our new prologue
            // as such must get range of search first before creating stub.
            nuint maxDisplacement      = Int32.MaxValue - searchLength;
            nuint newOriginalPrologue  = Utilities.InsertJump(remainingInstructions, _is64Bit, newAddress, originalJmpTarget, (nint)maxDisplacement);
            
            // Catch all return addresses in page range.
            var pageRange       = new AddressRange(initialSearchPointer, initialSearchPointer + searchLength);
            var jumpTargetRange = new AddressRange(startRemainingOpcodes, newAddress);

            patch.Patches = PatchJumpTargets(pageRange, originalJmpTarget, jumpTargetRange, newOriginalPrologue);
        }

        internal List<Patch> PatchJumpTargets(AddressRange searchRange, nuint originalJmpTarget, AddressRange jumpTargetRange, nuint newOriginalPrologue)
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
            var patchesForPage = PatchJumpTargets_Internal(searchRange, jumpTargetRange, newOriginalPrologue);
            result.AddRange(patchesForPage);
            return result;

            bool TryCodeAlignmentRange(AddressRange range)
            {
                // Clamp to current memory page if the start/end cannot be read.
                if (range.StartPointer < searchRange.StartPointer && Utilities.IsBadReadPtr(range.StartPointer.ToSigned()))
                    range.StartPointer = searchRange.StartPointer;

                if (range.EndPointer > searchRange.EndPointer && Utilities.IsBadReadPtr(range.EndPointer.ToSigned()))
                    range.EndPointer = searchRange.EndPointer;

                var patchesForImmediateArea = PatchJumpTargets_Internal(range, jumpTargetRange, newOriginalPrologue);
                result.AddRange(patchesForImmediateArea);
                return patchesForImmediateArea.Count > 0;
            }
        }

        /// <summary>
        /// Creates patch for a relative jump, if necessary.
        /// </summary>
        private void PatchRelativeJump(Instruction instruction, ref AddressRange originalJmpTarget, nuint newJmpTarget, List<Patch> patches)
        {
            nuint jumpTargetAddress = (nuint)GetJumpTarget(instruction);
            if (originalJmpTarget.Contains(jumpTargetAddress))
            {
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump((nuint)instruction.IP, newJmpTarget, _is64Bit);
                patches.Add(new Patch((nuint)instruction.IP, relativeJumpBytes));
            }
        }

        /// <summary>
        /// Creates patch for a RIP relative jump, if necessary.
        /// </summary>
        private void PatchRIPRelativeJump(Instruction instruction, ref AddressRange originalJmpTarget, nuint newJmpTarget, List<Patch> patches)
        {
            var jumpTargetAddress = GetRewriteRIPRelativeJumpTarget(instruction);

            if (originalJmpTarget.Contains(jumpTargetAddress))
            {
                // newJmpTarget is guaranteed to be in range.
                // Relative jump uses less bytes, so using it is also safe.
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump((nuint)instruction.IP, newJmpTarget, _is64Bit);
                patches.Add(new Patch((nuint)instruction.IP, relativeJumpBytes));
            }
        }

        /// <summary>
        /// Creates patch for a push + return combo, if necessary.
        /// </summary>
        private void PatchPushReturn(Instruction instruction, ref AddressRange originalJmpTarget, nuint newJmpTarget, List<Patch> patches)
        {
            ulong jumpTargetAddress = GetPushReturnTarget(instruction);

            if (originalJmpTarget.Contains((nuint)jumpTargetAddress))
            {
                // Push + Return & JMP Absolute use the same number of bytes in X86. but not in X64.
                // We must create a new Push + Return to an absolute jump.
                byte[] absoluteJump = Utilities.AssembleAbsoluteJump(newJmpTarget, _is64Bit);
                var buffer = Utilities.FindOrCreateBufferInRange(absoluteJump.Length);
                var absoluteJmpPointer = buffer.Add(absoluteJump);

                byte[] newPushReturn = Utilities.AssemblePushReturn(absoluteJmpPointer, _is64Bit);
                patches.Add(new Patch((nuint)instruction.IP, newPushReturn));
            }
        }

        /// <summary>
        /// [Part of PatchJumpTargets_Internal]
        /// Obtains the address range to perform search for jumps back by modifying a given searchPointer and giving a searchRange.
        /// </summary>
        /// <param name="searchPointer">The initial pointer from which to deduce the search range.</param>
        /// <param name="searchLength"> The length of the search.</param>
        internal void GetSearchRange(ref nuint searchPointer, out nuint searchLength)
        {
            searchLength = 0;

            // Search in modules (if necessary).
            if (_options.SearchInModules)
            {
                // Cache the module list.
                foreach (ProcessModule module in GetCachedModules())
                {
                    nuint minimumAddress = module.BaseAddress.ToUnsigned();
                    nuint maximumAddress = module.BaseAddress.ToUnsigned() + (nuint)module.ModuleMemorySize;

                    if (searchPointer >= minimumAddress && searchPointer <= maximumAddress)
                    {
                        searchPointer = minimumAddress;
                        searchLength = (nuint)module.ModuleMemorySize;
                    }
                }
            }

            // If the search range is 0 (our address is not in a module),
            // consider instead scanning the whole memory page.
            if (searchLength == 0)
            {
                searchLength = (nuint)Environment.SystemPageSize;
                searchPointer -= searchPointer % searchLength;
            }
        }

        /// <summary>
        /// Attempts to read out a number of bytes from unmanaged memory.
        /// </summary>
        /// <param name="address">Address to read from.</param>
        /// <param name="size">The size of memory.</param>
        private byte[] TryReadFromMemory(nuint address, int size)
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
            return instruction.Mnemonic == Mnemonic.Jmp &&
                   instruction.OpCount > 0 &&
                   (instruction.Op0Kind == OpKind.NearBranch16 ||
                    instruction.Op0Kind == OpKind.NearBranch32 ||
                    instruction.Op0Kind == OpKind.NearBranch64);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsPushReturn(Instruction instruction, Instruction nextInstruction)
        {
            return instruction.Mnemonic == Mnemonic.Push &&
                   instruction.OpCount >= 1 &&
                   instruction.Op0Kind is OpKind.Immediate32 or OpKind.Immediate32to64 && // Does not support 64bit immediates.
                   nextInstruction.Mnemonic == Mnemonic.Ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsRIPRelativeJump(Instruction instruction)
        {
            return instruction.Mnemonic == Mnemonic.Jmp &&
                   _is64Bit &&
                   instruction.OpCount >= 1 &&
                   instruction.IsIPRelativeMemoryOperand;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsJumpTargetInAModule(nuint source, nuint target)
        {
            if (!_options.VerifyJumpTargetsModule)
                return false;

            foreach (ProcessModule module in GetCachedModules())
            {
                var range = new AddressRange(module.BaseAddress.ToUnsigned(), (module.BaseAddress + module.ModuleMemorySize).ToUnsigned());
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
        private ulong GetJumpTarget(Instruction instruction)
        {
            return instruction.Op0Kind switch
            {
                OpKind.NearBranch16 => instruction.NearBranch16,
                OpKind.NearBranch32 => instruction.NearBranch32,
                OpKind.NearBranch64 => instruction.NearBranch64,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Obtains the offset of a relative immediate operand, else throws exception,
        /// </summary>
        private ulong GetOperandOffset(Instruction instruction)
        {
            return instruction.Op0Kind switch
            {
                OpKind.Immediate8 => instruction.Immediate8,
                OpKind.Immediate16 => instruction.Immediate16,
                OpKind.Immediate32 => instruction.Immediate32,
                OpKind.Immediate64 => instruction.Immediate64,
                OpKind.Immediate32to64 => (ulong)instruction.Immediate32to64,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private struct JumpDetails
        {
            /// <summary>
            /// Pointer to end of the opcode combination that causes the jump.
            /// </summary>
            public nuint JumpOpcodeEnd;

            /// <summary>
            /// Where the opcode jumps to.
            /// </summary>
            public nuint JumpOpcodeTarget;

            public JumpDetails(nuint jumpOpcodeEnd, nuint jumpOpcodeTarget)
            {
                JumpOpcodeEnd = jumpOpcodeEnd;
                JumpOpcodeTarget = jumpOpcodeTarget;
            }
        }

        /// <summary>
        /// Gets the address of the next instruction following an instruction.
        /// </summary>
        /// <param name="instruction">The instruction.</param>
        private static nuint PC(Instruction instruction) => (nuint)instruction.IP + (nuint)instruction.Length;
    }
}
