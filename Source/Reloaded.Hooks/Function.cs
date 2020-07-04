using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;

namespace Reloaded.Hooks
{
    /// <summary>
    /// Class that encapsulates a singular function, allowing for actions to be directly performed on that function.
    /// </summary>
    public class Function<TFunction> : IFunction<TFunction>
    {
        /// <summary>
        /// Address of the function in memory.
        /// </summary>
        public long Address { get; private set; }

        /// <summary>
        /// Provides an interface to the hooks library.
        /// </summary>
        public IReloadedHooks Hooks { get; private set; }

        private TFunction _wrapper;
        private IntPtr _wrapperAddress;

        /// <summary>
        /// Encapsulates a function.
        /// </summary>
        /// <param name="address">The address of the function in question.</param>
        /// <param name="hooks">Provides the hooking capability for this class.</param>
        public Function(long address, IReloadedHooks hooks)
        {
            Address = address;
            Hooks = hooks;
        }

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="function">The function to detour the original function to.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public IHook<TFunction> Hook(TFunction function, int minHookLength = -1)
        {
            return Hooks.CreateHook(function, Address, minHookLength);
        }

        /// <summary>
        /// Allows you to call this function as if it were a X86 CDECL/X64 Microsoft function.
        /// </summary>
        /// <remarks>The return value of this function is cached. Multiple calls will return same value.</remarks>
        public TFunction GetWrapper()
        {
            if (_wrapper == null) 
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out _wrapperAddress);

            return _wrapper;
        }

        /// <summary>
        /// Allows you to call this function as if it were a X86 CDECL/X64 Microsoft function.
        /// </summary>
        /// <param name="wrapperAddress">
        ///     Native address of the wrapper used to call the original function.
        ///     If the original function is X86 CDECL/X64 Microsoft, the wrapper address equals the function address.
        /// </param>
        /// <remarks>The return value of this function is cached. Multiple calls will return same value.</remarks>
        public TFunction GetWrapper(out IntPtr wrapperAddress)
        {
            if (_wrapper == null)
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out _wrapperAddress);

            wrapperAddress = _wrapperAddress;
            return _wrapper;
        }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">
        ///     The assembly code to execute, in FASM syntax.
        ///     (Should start with use32/use64)
        /// </param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public IAsmHook MakeAsmHook(string[] asmCode, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1)
        {
            return Hooks.CreateAsmHook(asmCode, Address, behaviour, hookLength);
        }

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public IAsmHook MakeAsmHook(byte[] asmCode, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1)
        {
            return Hooks.CreateAsmHook(asmCode, Address, behaviour, hookLength);
        }

        #if FEATURE_FUNCTION_POINTERS
        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="functionPtr">Pointer to the function to detour the original to.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public unsafe IHook<TFunction, TFunctionPointer> Hook<TFunctionPointer>(void* functionPtr, int minHookLength = -1) where TFunctionPointer : unmanaged 
        {
            return Hooks.CreateHook<TFunction, TFunctionPointer>(functionPtr, Address, minHookLength);
        }

        /// <summary>
        /// Gets the address of a wrapper function in memory that allows you to call this function as if it were a X86 CDECL/X64 Microsoft function.
        /// </summary>
        /// <remarks>The return value of this function is cached. Multiple calls will return same value.</remarks>
        public IntPtr GetWrapperPtr() 
        {
            GetWrapper(out var address);
            return address;
        }

        /// <summary>
        /// Gets the address of a wrapper function in memory that allows you to call this function as if it were a X86 CDECL/X64 Microsoft function.
        /// </summary>
        /// <remarks>The return value of this function is cached. Multiple calls will return same value.</remarks>
        public TFunctionPointer GetWrapperPtr<TFunctionPointer>()
        {
            var ptr = GetWrapperPtr();
            return Unsafe.As<IntPtr, TFunctionPointer>(ref ptr);
        }
        #endif
    }
}
