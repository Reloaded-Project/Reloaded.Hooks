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
        /// <inheritdoc />
        public long Address { get; private set; }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public IHook<TFunction> Hook(TFunction function, int minHookLength) => Hooks.CreateHook(function, Address, minHookLength);

        /// <inheritdoc />
        public IHook<TFunction> Hook(TFunction function) => Hook(function, -1);

        /// <inheritdoc />
        public unsafe IHook<TFunction> Hook(void* function, int minHookLength) => Hooks.CreateHook<TFunction>(function, Address, minHookLength);

        /// <inheritdoc />
        public unsafe IHook<TFunction> Hook(void* function) => Hook(function, -1);

        /// <inheritdoc />
        public unsafe IHook<TFunction> Hook(Type type, string methodName, int minHookLength) => Hooks.CreateHook<TFunction>(type, methodName, Address, minHookLength);

        /// <inheritdoc />
        public unsafe IHook<TFunction> Hook(Type type, string methodName) => Hook(type, methodName, -1);

        /// <inheritdoc />
        public unsafe IHook<TFunctionType> HookAs<TFunctionType>(void* function, int minHookLength) => Hooks.CreateHook<TFunctionType>(function, Address, minHookLength);

        /// <inheritdoc />
        public unsafe IHook<TFunctionType> HookAs<TFunctionType>(void* function) => HookAs<TFunctionType>(function, -1);

        /// <inheritdoc />
        public unsafe IHook<TFunctionType> HookAs<TFunctionType>(Type type, string methodName, int minHookLength) => Hooks.CreateHook<TFunctionType>(type, methodName, Address, minHookLength);

        /// <inheritdoc />
        public unsafe IHook<TFunctionType> HookAs<TFunctionType>(Type type, string methodName) => HookAs<TFunctionType>(type, methodName, -1);

        /// <inheritdoc />
        public TFunction GetWrapper()
        {
            if (_wrapper == null) 
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out _wrapperAddress);

            return _wrapper;
        }

        /// <inheritdoc />
        public TFunction GetWrapper(out IntPtr wrapperAddress)
        {
            if (_wrapper == null)
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out _wrapperAddress);

            wrapperAddress = _wrapperAddress;
            return _wrapper;
        }

        /// <inheritdoc />
        public IAsmHook MakeAsmHook(string[] asmCode, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1)
        {
            return Hooks.CreateAsmHook(asmCode, Address, behaviour, hookLength);
        }

        /// <inheritdoc />
        public IAsmHook MakeAsmHook(byte[] asmCode, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1)
        {
            return Hooks.CreateAsmHook(asmCode, Address, behaviour, hookLength);
        }
    }
}
