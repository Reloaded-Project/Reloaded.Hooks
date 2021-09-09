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

        private const int MaxRelJmpSize = 5; // Maximum size of jmp opcode.
        private const int MaxAbsJmpSize = 7; // Maximum size of jmp opcode.

        /// <inheritdoc />
        public bool IsEnabled { get; private set; } = false;

        private Patch _activateHookPatch;
        private Patch _disableHookPatch;
        private Patch _enableHookPatch;
        private bool _activated = false;
        private bool _is64Bit;

        /* Construction - Destruction */

        private AsmHook()
        {
            _is64Bit = sizeof(IntPtr) == 8;
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
        public AsmHook(string[] asmCode, long functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1) 
            : this(Utilities.Assembler.Assemble(asmCode), functionAddress, new AsmHookOptions() { Behaviour = behaviour, hookLength = hookLength })
        { }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public AsmHook(byte[] asmCode, long functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1) : this(asmCode, functionAddress,
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
        public AsmHook(string[] asmCode, long functionAddress, AsmHookOptions options = default) : this(Utilities.Assembler.Assemble(asmCode), functionAddress, options)
        { }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="options">The options used for creating the assembly hook.</param>
        public AsmHook(byte[] asmCode, long functionAddress, AsmHookOptions options = default) : this()
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
                options.hookLength = Utilities.GetHookLength((IntPtr)functionAddress, options.MaxOpcodeSize, _is64Bit);

            CurrentProcess.SafeReadRaw((IntPtr)functionAddress, out byte[] originalFunction, options.hookLength);
            long jumpBackAddress = functionAddress + options.hookLength;

            /* Size calculations for buffer, must have sufficient space. */

            // Stubs:
            // Stub Entry   => Stub Hook.
            // Stub Hook    => Caller.
            // Disable.Original Stub => Caller.

            int codeAlignment          = 16; // Alignment of code in memory.
            int numberOfStubs          = 3;  // Also number of allocations.
            int alignmentRequiredBytes = (codeAlignment * numberOfStubs);

            int pointerSize            = (_is64Bit ? 8 : 4);

            int pointerRequiredBytes   = pointerSize * 2; // 2 calls to AssembleAbsoluteJump
            int stubEntrySize          = MaxAbsJmpSize;
            int stubHookSize           = asmCode.Length + options.hookLength + MaxAbsJmpSize;
            int stubOriginalSize       = options.hookLength + MaxAbsJmpSize + pointerSize; // 1 call to AssembleAbsoluteJump

            int requiredSizeOfBuffer   = stubEntrySize + stubHookSize + stubOriginalSize + alignmentRequiredBytes + pointerRequiredBytes;
            var minMax                 = Utilities.GetRelativeJumpMinMax(functionAddress, Int32.MaxValue - requiredSizeOfBuffer);
            var buffer                 = Utilities.FindOrCreateBufferInRange(requiredSizeOfBuffer, minMax.min, minMax.max);

            buffer.ExecuteWithLock(() =>
            {
                var patcher             = new IcedPatcher(_is64Bit, originalFunction, (IntPtr) functionAddress);

                // Make Hook and Original Stub
                buffer.SetAlignment(codeAlignment);
                IntPtr hookStubAddr     = MakeHookStub(buffer, patcher, asmCode, originalFunction, jumpBackAddress, options.Behaviour);

                buffer.SetAlignment(codeAlignment);
                IntPtr originalStubAddr = MakeOriginalStub(buffer, patcher, originalFunction, jumpBackAddress);

                // Make Jump to Entry, Original Stub
                buffer.SetAlignment(codeAlignment);
                var currAddress = buffer.Properties.WritePointer;
                byte[] jmpToOriginal = Utilities.AssembleRelativeJump(currAddress, originalStubAddr, _is64Bit);
                byte[] jmpToHook     = Utilities.AssembleRelativeJump(currAddress, hookStubAddr, _is64Bit);

                // Make Entry Stub
                IntPtr entryStubAddr = buffer.Add(jmpToHook, codeAlignment);

                // Make Disable/Enable
                _disableHookPatch = new Patch(entryStubAddr, jmpToOriginal);
                _enableHookPatch  = new Patch(entryStubAddr, jmpToHook);

                // Make Hook Enabler
                var jumpOpcodes = options.PreferRelativeJump ? Utilities.AssembleRelativeJump((IntPtr) functionAddress, entryStubAddr, _is64Bit).ToList() : Utilities.AssembleAbsoluteJump(entryStubAddr, _is64Bit).ToList();
                Utilities.FillArrayUntilSize<byte>(jumpOpcodes, 0x90, options.hookLength);
                _activateHookPatch = new Patch((IntPtr) functionAddress, jumpOpcodes.ToArray());
                return true;
            });
        }

        private IntPtr MakeHookStub(MemoryBuffer buffer, IcedPatcher patcher, byte[] asmCode, byte[] originalCode, long jumpBackAddress, AsmHookBehaviour behaviour)
        {
            var bytes        = new List<byte>(asmCode.Length + originalCode.Length);
           
            switch (behaviour)
            {
                case AsmHookBehaviour.ExecuteFirst:
                    bytes.AddRange(asmCode);
                    bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer + bytes.Count));
                    break;

                case AsmHookBehaviour.ExecuteAfter:
                    bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer + bytes.Count));
                    bytes.AddRange(asmCode);
                    break;

                case AsmHookBehaviour.DoNotExecuteOriginal:
                    bytes.AddRange(asmCode);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null);
            }

            var jmpBackBytes = Utilities.AssembleRelativeJump(buffer.Properties.WritePointer + bytes.Count, (IntPtr)jumpBackAddress, _is64Bit);
            bytes.AddRange(jmpBackBytes);
            return buffer.Add(bytes.ToArray(), 1); // Buffer is pre-aligned
        }

        private IntPtr MakeOriginalStub(MemoryBuffer buffer, IcedPatcher patcher, byte[] originalCode, long jumpBackAddress)
        {
            var bytes         = new List<byte>(originalCode.Length);
            bytes.AddRange(patcher.EncodeForNewAddress(buffer.Properties.WritePointer));

            var jmpBackBytes = Utilities.AssembleRelativeJump(buffer.Properties.WritePointer + bytes.Count, (IntPtr)jumpBackAddress, _is64Bit);
            bytes.AddRange(jmpBackBytes);
            return buffer.Add(bytes.ToArray(), 1); // Buffer is pre-aligned
        }

        /* User Functionality */

        /// <inheritdoc />
        public IAsmHook Activate()
        {
            if (!_activated)
            {
                _activated = true;
                _activateHookPatch.Apply();
                _enableHookPatch.ApplyUnsafe();
            }

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
