using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.Helpers;

namespace Reloaded.Hooks
{
    /// <summary>
    /// Class that encapsulates a singular function, allowing for actions to be directly performed on that function.
    /// </summary>
    public class Function<TFunction> : IFunction<TFunction>
    {
        /// <inheritdoc />
        public long Address => _address.ToSigned();

        /// <inheritdoc />
        public IReloadedHooks Hooks { get; private set; }

        private bool _wrapperCreated = false;
        private TFunction _wrapper;
        private nuint _wrapperAddress;
        private nuint _address;

        /// <summary>
        /// Encapsulates a function.
        /// </summary>
        /// <param name="address">The address of the function in question.</param>
        /// <param name="hooks">Provides the hooking capability for this class.</param>
        public Function(nuint address, IReloadedHooks hooks)
        {
            _address = address;
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
            if (!_wrapperCreated)
            {
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out var wrapperAddress);
                _wrapperAddress = wrapperAddress.ToUnsigned();
                _wrapperCreated = true;
            }

            return _wrapper;
        }

        /// <inheritdoc />
        public TFunction GetWrapper(out IntPtr wrapperAddress)
        {
            if (!_wrapperCreated)
            {
                _wrapper = Hooks.CreateWrapper<TFunction>(Address, out var wrapperAddr);
                _wrapperAddress = wrapperAddr.ToUnsigned();
                _wrapperCreated = true;
            }

            wrapperAddress = _wrapperAddress.ToSigned();
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
