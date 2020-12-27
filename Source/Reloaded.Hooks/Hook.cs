using System;
using System.Collections.Generic;
using System.Linq;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;
using Reloaded.Hooks.X86;
using Reloaded.Memory.Sources;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks
{
    /// <summary>
    /// Specialized version of <see cref="Hook{TFunction, TFuncPointer}"/> with support for function pointers.
    /// </summary>
    public class Hook<TFunction, TFuncPointer> : Hook<TFunction>, IHook<TFunction, TFuncPointer> where TFuncPointer : unmanaged
    {
        public unsafe Hook(TFunction function, long functionAddress, int minHookLength = -1) : base(function, functionAddress, minHookLength)
        {
            OriginalFunction = UnsafeCastPointer<TFuncPointer>(OriginalFunctionWrapperAddress);
        }

        public unsafe Hook(void* targetAddress, long functionAddress, int minHookLength = -1) : base(targetAddress, functionAddress, minHookLength)
        {
            OriginalFunction = UnsafeCastPointer<TFuncPointer>(OriginalFunctionWrapperAddress);
        }

        /// <inheritdoc/>
        public new TFuncPointer OriginalFunction { get; }

        /// <inheritdoc/>
        public new IHook<TFunction, TFuncPointer> Activate() => (IHook<TFunction, TFuncPointer>) base.Activate();

        private static unsafe TPointer UnsafeCastPointer<TPointer>(IntPtr value) where TPointer : unmanaged
        {
            var address = value;
            return System.Runtime.CompilerServices.Unsafe.As<IntPtr, TPointer>(ref address);
        }
    }

    public class Hook<TFunction> : IHook<TFunction>
    {
        /// <inheritdoc />
        public bool IsHookEnabled { get; private set; } = false;

        /// <inheritdoc />
        public bool IsHookActivated { get; private set; } = false;

        /// <inheritdoc />
        public TFunction OriginalFunction { get; private set; }

        /// <inheritdoc />
        public IntPtr OriginalFunctionAddress { get; private set; }

        /// <inheritdoc />
        public IntPtr OriginalFunctionWrapperAddress { get; private set; }

        /// <inheritdoc />
        public IReverseWrapper<TFunction> ReverseWrapper { get; private set; }

        /* Patch which activates the current hook & rewrites other hooks' return addresses. */
        private Patch           _hookPatch;
        private List<Patch>     _otherHookPatches;

        private Patch           _disableHookPatch;
        private Patch           _enableHookPatch;
        private bool            _is64Bit;

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="function">The function to detour the original function to.</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public unsafe Hook(TFunction function, long functionAddress, int minHookLength = -1)
        {
            _is64Bit = sizeof(IntPtr) == 8;
            ReverseWrapper = CreateReverseWrapper(function);
            CreateHook(functionAddress, minHookLength);
        }

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="targetAddress">Address of the function to detour the original function to.</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public unsafe Hook(void* targetAddress, long functionAddress, int minHookLength = -1)
        {
            _is64Bit = sizeof(IntPtr) == 8;
            ReverseWrapper = CreateReverseWrapper(targetAddress);
            CreateHook(functionAddress, minHookLength);
        }

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        private void CreateHook(long functionAddress, int minHookLength = -1)
        {
            /*
               === Hook Summary ===

               A. Insert Absolute Jump to ReverseWrapper (Convention => CDECL Marshaller)
                   A1. Backup original bytes and patch between start and end of JMP for (B).

               B. Setup Wrapper to call original function (CDECL => Convention Marshaller)
                   B1. Take bytes backed up from A, and create stub function with those 
                       bytes and JMP to end of hook.
                   B2. Assign OriginalFunction to that function stub.

               Note: For X64 the same principles apply, just replace CDECL with Microsoft calling convention.  
            */

            /* Create Target Convention => TFunction Wrapper. */
            List<byte> jumpOpcodes = Utilities.AssembleAbsoluteJump(ReverseWrapper.WrapperPointer, _is64Bit).ToList();

            /* Calculate Hook Length (Unless Explicit) */
            if (minHookLength == -1)
                minHookLength = Utilities.GetHookLength((IntPtr)functionAddress, jumpOpcodes.Count, _is64Bit);

            // Sometimes our hook can be larger than the amount of bytes taken by the jmp opcode.
            // We need to fill the remaining bytes with NOPs.
            Utilities.FillArrayUntilSize<byte>(jumpOpcodes, 0x90, minHookLength);

            /* Get bytes from original function prologue and patch them. */
            CurrentProcess.SafeReadRaw((IntPtr)functionAddress, out byte[] originalFunction, minHookLength);

            var functionPatcher   = new FunctionPatcher(_is64Bit);
            var functionPatch     = functionPatcher.Patch(originalFunction.ToList(), (IntPtr)functionAddress);
            IntPtr hookEndAddress = (IntPtr)(functionAddress + minHookLength);

            /* Second wave of patching. */
            var icedPatcher = new IcedPatcher(_is64Bit, functionPatch.NewFunction.ToArray(), (IntPtr)functionAddress);

            /* Create Hook instance. */
            OriginalFunctionAddress = icedPatcher.ToMemoryBuffer(hookEndAddress);
            OriginalFunction = CreateWrapper((long)icedPatcher.ToMemoryBuffer(null), out IntPtr originalFunctionWrapperAddress);
            OriginalFunctionWrapperAddress = originalFunctionWrapperAddress;

            _otherHookPatches = functionPatch.Patches;
            _hookPatch = new Patch((IntPtr)functionAddress, jumpOpcodes.ToArray());
        }

        /// <inheritdoc />
        public IHook<TFunction> Activate()
        {
            /* Create enable/disable patch. */
            var disableOpCodes = Utilities.AssembleAbsoluteJump(OriginalFunctionAddress, _is64Bit);
            CurrentProcess.SafeReadRaw(ReverseWrapper.WrapperPointer, out var originalOpcodes, disableOpCodes.Length);
            _disableHookPatch = new Patch(ReverseWrapper.WrapperPointer, disableOpCodes);
            _enableHookPatch = new Patch(ReverseWrapper.WrapperPointer, originalOpcodes);

            /* Activate the hook. */
            _hookPatch.Apply();

            foreach (var hookPatch in _otherHookPatches)
                hookPatch.Apply();

            /* Set flags. */
            IsHookEnabled = true;
            IsHookActivated = true;

            return this;
        }

        /// <inheritdoc />
        IHook IHook.Activate() => Activate();

        /// <inheritdoc />
        public void Disable()
        {
            if (IsHookActivated)
            {
                _disableHookPatch.ApplyUnsafe();
                IsHookEnabled = false;
            }
        }

        /// <inheritdoc />
        public void Enable()
        {
            if (IsHookActivated)
            {
                _enableHookPatch.ApplyUnsafe();
                IsHookEnabled = true;
            }
        }

        protected IReverseWrapper<TFunction> CreateReverseWrapper(TFunction function)
        {
            if (_is64Bit)
                return new X64.ReverseWrapper<TFunction>(function);

            return new ReverseWrapper<TFunction>(function);
        }

        protected unsafe IReverseWrapper<TFunction> CreateReverseWrapper(void* function)
        {
            if (_is64Bit)
                return new X64.ReverseWrapper<TFunction>((IntPtr) function);

            return new ReverseWrapper<TFunction>((IntPtr) function);
        }

        protected TFunction CreateWrapper(long functionAddress, out IntPtr wrapperAddress)
        {
            if (_is64Bit)
                return X64.Wrapper.Create<TFunction>(functionAddress, out wrapperAddress);

            return Wrapper.Create<TFunction>(functionAddress, out wrapperAddress);
        }
    }
}
