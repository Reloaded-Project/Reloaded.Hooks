using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Buffers;
using Reloaded.Memory.Sources;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks
{
    /// <summary>
    /// A more barebones Cheat Engine-like hook for advanced scenarios.
    /// </summary>
    public unsafe class AsmHook : IAsmHook
    {
        private static Memory.Sources.Memory _memory = CurrentProcess;

        /// <inheritdoc />
        public bool IsEnabled { get; private set; } = false;

        private Patch _activateHookPatch;
        private Patch _disableHookPatch;
        private Patch _enableHookPatch;
        private bool _activated = false;
        private readonly bool _is64Bit;
        
        private List<Patch>? _otherHookPatches;

        /* Construction - Destruction */

        private AsmHook()
        {
            _is64Bit = sizeof(nuint) == 8;
        }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">
        ///     The assembly code to execute, in FASM syntax.
        ///     (Should start with use32/use64)
        /// </param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public AsmHook(string[] asmCode, nuint functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1) 
            : this(Utilities.Assembler.Assemble(asmCode), functionAddress, new AsmHookOptions() { Behaviour = behaviour, hookLength = hookLength })
        { }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public AsmHook(byte[] asmCode, nuint functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1) : this(asmCode, functionAddress,
                new AsmHookOptions() { Behaviour = behaviour, hookLength = hookLength })
        { }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">
        ///     The assembly code to execute, in FASM syntax.
        ///     (Should start with use32/use64)
        /// </param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="options">The options used for creating the assembly hook.</param>
        public AsmHook(string[] asmCode, nuint functionAddress, AsmHookOptions options = default) : this(Utilities.Assembler.Assemble(asmCode), functionAddress, options)
        { }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="options">The options used for creating the assembly hook.</param>
        public AsmHook(byte[] asmCode, nuint functionAddress, AsmHookOptions options = default) : this()
        {
            options ??= new AsmHookOptions();

            /*
               === Hook Summary ===

               A. Backup Original Code & Generate Function Disable Stub
                   A1. Find amount of bytes required to insert JMP opcode, backup original bytes.
                   B2. Function disable stub will contain original code and jump back to end of hook.

               B. Generate Function Stub. 
                   B1. For stub, generate code combining asmCode and original instructions, depending on AsmHookBehaviour
                   B2. Add jmp back to end of original hook.

               Graph:
                   Original => Stub Entry (JMP To Hook or Original Copy) => Hook/Original Copy with JMP back. 

               Notes: For hook disable, replace jmp address to function disable stub.
                      For hook re-enable, replace jmp address to function hook stub.
            */


            if (options.hookLength == -1)
                options.hookLength = Utilities.GetHookLength(functionAddress, options.MaxOpcodeSize, _is64Bit);

            CurrentProcess.SafeReadRaw(functionAddress, out byte[] originalFunction, options.hookLength);

            nuint jumpBackAddress = functionAddress + (nuint)options.hookLength;

            /* Size calculations for buffer, must have sufficient space. */

            // Theoretical largest possible expansion is 2 byte jmp to 6 byte jmp,
            // Which means an increase in 4 bytes per 2 bytes.
            int exceptionHandlingBytes = ((originalFunction.Length / 2) + 1) * 4;
            
            // Stubs:
            // Stub Entry   => Stub Hook.
            // Stub Hook    => Caller.
            // Disable.Original Stub => Caller.

            int codeAlignment          = 16; // Alignment of code in memory.
            int numberOfStubs          = 3;  // Also number of allocations.
            int alignmentRequiredBytes = (codeAlignment * numberOfStubs);

            int pointerSize            = (_is64Bit ? 8 : 4);

            int pointerRequiredBytes   = pointerSize * 2; // 2 calls to AssembleAbsoluteJump
            int stubEntrySize          = Constants.MaxAbsJmpSize;
            int stubHookSize           = asmCode.Length + options.hookLength + Constants.MaxAbsJmpSize;
            int stubOriginalSize       = options.hookLength + Constants.MaxAbsJmpSize + pointerSize; // 1 call to AssembleAbsoluteJump

            int requiredSizeOfBuffer   = stubEntrySize + stubHookSize + stubOriginalSize + alignmentRequiredBytes + pointerRequiredBytes + exceptionHandlingBytes;
            var minMax                 = Utilities.GetRelativeJumpMinMax(functionAddress, Int32.MaxValue - requiredSizeOfBuffer);
            var buffer                 = Utilities.FindOrCreateBufferInRange(requiredSizeOfBuffer, minMax.min, minMax.max);

            buffer.ExecuteWithLock(() =>
            {
                try
                {
                    return MakeAsmHook(asmCode, functionAddress, options, originalFunction, buffer, codeAlignment, jumpBackAddress);
                }
                catch (Exception)
                {
                    // In very exceptional case, we can't re-encode due to another hook, so rewrite as absolute.
                    var functionPatcher = new FunctionPatcher(_is64Bit);
                    var functionPatch   = functionPatcher.Patch(originalFunction.ToList(), functionAddress);
                    originalFunction    = functionPatch.NewFunction.ToArray();
                    _otherHookPatches   = functionPatch.Patches;
                    return MakeAsmHook(asmCode, functionAddress, options, originalFunction, buffer, codeAlignment, jumpBackAddress);
                }
            });
        }

        private bool MakeAsmHook(byte[] asmCode, nuint functionAddress, AsmHookOptions options, byte[] originalFunction, MemoryBuffer buffer, int codeAlignment, nuint jumpBackAddress)
        {
            var patcher = new IcedPatcher(_is64Bit, originalFunction, functionAddress);

            // Make Hook and Original Stub
            buffer.SetAlignment(codeAlignment);
            nuint hookStubAddr = MakeHookStub(buffer, patcher, asmCode, originalFunction.Length, jumpBackAddress, options.Behaviour);

            buffer.SetAlignment(codeAlignment);
            nuint originalStubAddr = MakeOriginalStub(buffer, patcher, originalFunction.Length, jumpBackAddress);

            // Make Jump to Entry, Original Stub
            buffer.SetAlignment(codeAlignment);
            var currAddress = buffer.Properties.WritePointer;
            byte[] jmpToOriginal = Utilities.AssembleRelativeJump(currAddress, originalStubAddr, _is64Bit);
            byte[] jmpToHook = Utilities.AssembleRelativeJump(currAddress, hookStubAddr, _is64Bit);

            // Make Entry Stub
            nuint entryStubAddr = buffer.Add(jmpToHook, codeAlignment);

            // Make Disable/Enable
            _disableHookPatch = new Patch(entryStubAddr, jmpToOriginal);
            _enableHookPatch = new Patch(entryStubAddr, jmpToHook);

            // Make Hook Enabler
            var jumpOpcodes = options.PreferRelativeJump
                ? Utilities.AssembleRelativeJump(functionAddress, entryStubAddr, _is64Bit).ToList()
                : Utilities.AssembleAbsoluteJump(entryStubAddr, _is64Bit).ToList();
            Utilities.FillArrayUntilSize<byte>(jumpOpcodes, 0x90, options.hookLength);
            _activateHookPatch = new Patch(functionAddress, jumpOpcodes.ToArray());
            return true;
        }

        private nuint MakeHookStub(MemoryBuffer buffer, IcedPatcher patcher, byte[] asmCode, int originalCodeLength, nuint jumpBackAddress, AsmHookBehaviour behaviour)
        {
            var bytes        = new List<byte>(asmCode.Length + originalCodeLength);
           
            switch (behaviour)
            {
                case AsmHookBehaviour.ExecuteFirst:
                    bytes.AddRange(asmCode);
                    bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer + (nuint)bytes.Count));
                    break;

                case AsmHookBehaviour.ExecuteAfter:
                    bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer + (nuint)bytes.Count));
                    bytes.AddRange(asmCode);
                    break;

                case AsmHookBehaviour.DoNotExecuteOriginal:
                    bytes.AddRange(asmCode);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }

            var jmpBackBytes = Utilities.AssembleRelativeJump(buffer.Properties.WritePointer + (nuint)bytes.Count, jumpBackAddress, _is64Bit);
            bytes.AddRange(jmpBackBytes);
            return buffer.Add(bytes.ToArray(), 1); // Buffer is pre-aligned
        }

        private nuint MakeOriginalStub(MemoryBuffer buffer, IcedPatcher patcher, int originalCodeLength, nuint jumpBackAddress)
        {
            var bytes         = new List<byte>(originalCodeLength);
            bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer));

            var jmpBackBytes = Utilities.AssembleRelativeJump(buffer.Properties.WritePointer + (nuint)bytes.Count, jumpBackAddress, _is64Bit);
            bytes.AddRange(jmpBackBytes);
            return buffer.Add(bytes.ToArray(), 1); // Buffer is pre-aligned
        }

        /* User Functionality */

        /// <inheritdoc />
        public IAsmHook Activate()
        {
            if (_activated) 
                return this;
            
            _activated = true;
            _activateHookPatch.Apply();
            _enableHookPatch.ApplyUnsafe();

            if (_otherHookPatches == null) 
                return this;
            
            foreach (var hookPatch in _otherHookPatches)
                hookPatch.Apply();

            return this;
        }

        /// <inheritdoc />
        public void Enable()
        {
            _enableHookPatch.ApplyUnsafe();
            IsEnabled = true;
        }

        /// <inheritdoc />
        public void Disable()
        {
            _disableHookPatch.ApplyUnsafe();
            IsEnabled = false;
        }
    }
}
