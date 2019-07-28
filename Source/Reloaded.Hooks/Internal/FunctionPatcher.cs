using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using Reloaded.Hooks.Tools;
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

        public FunctionPatcher(ArchitectureMode mode)
        {
            _architecture = mode;
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

            Mutex.FunctionPatcherMutex.WaitOne();

            FunctionPatch functionPatch = new FunctionPatch();
            long reloadedHookEndAddress = oldFunction.Count + (long)baseAddress; // End of our own hook.

            Disassembler  disassembler = new Disassembler(oldFunction.ToArray(), _architecture, (ulong)baseAddress, true);
            Instruction[] instructions = disassembler.Disassemble().ToArray();
            
            for (int x = 0; x < instructions.Length; x++)
            {
                Instruction instruction     = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Length) ? instructions[x + 1] : null;
                JumpDetails jumpDetails;

                if      (IsRelativeJump(instruction) && IsJumpTargetInDifferentModule((long) instruction.PC, GetRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRelativeJump(instruction, functionPatch);
                else if (IsRIPRelativeJump(instruction) && IsJumpTargetInDifferentModule((long)instruction.PC, (long)GetRewriteRIPRelativeJumpTarget(instruction)) )
                    jumpDetails = RewriteRIPRelativeJump(instruction, functionPatch);
                else if (nextInstruction != null && IsPushReturn(instruction, nextInstruction) && IsJumpTargetInDifferentModule((long)instruction.PC, GetPushReturnTarget(instruction)) )
                    jumpDetails = RewritePushReturn(instruction, nextInstruction, functionPatch);
                else
                {
                    functionPatch.NewFunction.AddRange(instruction.Bytes);
                    continue; // No patching on no addresses to patch.
                }

                PatchReturnAddresses(jumpDetails, functionPatch, reloadedHookEndAddress);
            }

            Mutex.FunctionPatcherMutex.ReleaseMutex();
            return functionPatch;
        }

        /// <summary>
        /// Patches all jumps pointing to originalJmpTarget to point to newJmpTarget.
        /// </summary>
        /// <param name="searchRange">Range of addresses where to patch jumps.</param>
        /// <param name="originalJmpTarget">Address range of JMP targets to patch with newJmpTarget.</param>
        /// <param name="newJmpTarget">The new address instructions should jmp to.</param>
        internal List<Patch> PatchJumpTargets(AddressRange searchRange, AddressRange originalJmpTarget, long newJmpTarget)
        {
            var patches = new List<Patch>();

            int length  = (int) (searchRange.EndPointer - searchRange.StartPointer);
            CurrentProcess.SafeReadRaw((IntPtr)searchRange.StartPointer, out byte[] memory, length);

            Disassembler  disassembler = new Disassembler(memory, _architecture, (ulong)searchRange.StartPointer, true);
            Instruction[] instructions = disassembler.Disassemble().ToArray();

            for (int x = 0; x < instructions.Length; x++)
            {
                Instruction instruction = instructions[x];
                Instruction nextInstruction = (x + 1 < instructions.Length) ? instructions[x + 1] : null;

                if (IsRelativeJump(instruction))
                    PatchRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                if (IsRIPRelativeJump(instruction))
                    PatchRIPRelativeJump(instruction, ref originalJmpTarget, newJmpTarget, patches);
                if (IsPushReturn(instruction, nextInstruction))
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
            long originalJmpTarget = jumpDetails.JumpOpcodeTarget;
            GetSearchRange(ref originalJmpTarget, out long searchLength);

            /* Get original opcodes after original JMP instruction. */

            IntPtr startRemainingOpcodes = (IntPtr)jumpDetails.JumpOpcodeEnd;
            int lengthRemainingOpcodes   = (int)(newAddress - (long)startRemainingOpcodes);
            CurrentProcess.ReadRaw(startRemainingOpcodes, out byte[] remainingInstructions, lengthRemainingOpcodes);

            /* Build function stub + patches. */

            // Must guarantee relative jumps to be patches can reach our new prologue
            // as such must get range of search first before creating stub.
            long maxDisplacement         = Int32.MaxValue - searchLength;
            IntPtr newOriginalPrologue   = Utilities.InsertJump(remainingInstructions, Is64Bit(), newAddress, originalJmpTarget, maxDisplacement);

            var patches = PatchJumpTargets(new AddressRange(originalJmpTarget, originalJmpTarget + searchLength),
                                           new AddressRange((long)startRemainingOpcodes, newAddress), (long) newOriginalPrologue);

            patch.Patches.AddRange(patches);
        }


        /// <summary>
        /// Creates patch for a relative jump, if necessary.
        /// </summary>
        private void PatchRelativeJump(Instruction instruction, ref AddressRange originalJmpTarget, long newJmpTarget, List<Patch> patches)
        {
            long jumpTargetAddress = (long)instruction.PC + GetOperandOffset(instruction.Operands[0]);
            if (originalJmpTarget.Contains(jumpTargetAddress))
            {
                // Edit: It's actually correct because FASM automatically adjusts for instruction length.
                IntPtr newRelativeOffset = (IntPtr)(newJmpTarget - (long)instruction.Offset);
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump(newRelativeOffset, Is64Bit());
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
                IntPtr newRelativeOffset = (IntPtr)(newJmpTarget - (long)instruction.Offset);
                byte[] relativeJumpBytes = Utilities.AssembleRelativeJump(newRelativeOffset, Is64Bit());
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
        /// [Part of PatchJumpTargets]
        /// Obtains the address range to perform search for jumps back by modifying a given searchPointer and giving a searchRange.
        /// </summary>
        /// <param name="searchPointer">The initial pointer from which to deduce the search range.</param>
        /// <param name="searchLength"> The length of the search.</param>
        internal void GetSearchRange(ref long searchPointer, out long searchLength)
        {
            searchLength = 0;

            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                long minimumAddress = (long)module.BaseAddress;
                long maximumAddress = (long)module.BaseAddress + module.ModuleMemorySize;

                if (searchPointer >= minimumAddress && searchPointer <= maximumAddress)
                {
                    searchPointer = minimumAddress;
                    searchLength = module.ModuleMemorySize;
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
        private bool IsJumpTargetInDifferentModule(long source, long target)
        {
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                var range = new AddressRange((long) module.BaseAddress, (long) (module.BaseAddress + module.ModuleMemorySize));
                if (range.Contains(source) && range.Contains(target))
                    return false;
            }

            return true;
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
