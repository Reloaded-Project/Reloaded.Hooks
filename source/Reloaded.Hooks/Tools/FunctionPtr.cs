using System;
using System.Collections.Generic;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;

namespace Reloaded.Hooks.Tools
{
    /// <summary>
    /// Represents a native function pointer.
    /// </summary>
    public unsafe class FunctionPtr<TDelegate> : IFunctionPtr<TDelegate> where TDelegate : Delegate
    {
        /// <inheritdoc />
        public ulong FunctionPointer { get; }
        
        /// <summary> Cache of already created function wrappers for functions at address. </summary>
        private readonly Dictionary<IntPtr, TDelegate> _methodCache;

        /// <summary> Caches the last used delegate. </summary>
        private TDelegate _delegate;

        /// <summary> Contains the address of the last called function. </summary>
        private IntPtr _lastFunctionPointer;

        /// <inheritdoc />
        public TDelegate this[int index] => GetDelegate(index);

        /// <summary>
        /// Abstracts a pointer to a native function.
        /// </summary>
        public FunctionPtr(ulong functionPointer)
        {
            FunctionPointer = functionPointer;
            _methodCache = new Dictionary<IntPtr, TDelegate>();
        }

        /// <inheritdoc />
        public IntPtr GetFunctionAddress(int index)
        {
            IntPtr* functionPtr = (IntPtr*) FunctionPointer;
            return functionPtr[index];
        }

        /// <inheritdoc />
        public TDelegate GetDelegate(int index = 0)
        {
            IntPtr functionPointer = GetFunctionAddress(index);
            
            if (functionPointer == _lastFunctionPointer)
                return _delegate;

            // Try to get the cached function wrapper.
            if (_methodCache.TryGetValue(functionPointer, out var cachedDelegate))
            {
                return cachedDelegate;
            }

            if (sizeof(IntPtr) == 4)
                _delegate = X86.Wrapper.Create<TDelegate>(functionPointer.ToUnsigned(), out var wrapperAddress);
            else
                _delegate = X64.Wrapper.Create<TDelegate>(functionPointer.ToUnsigned(), out var wrapperAddress);

            _methodCache[functionPointer] = _delegate;
            _lastFunctionPointer = functionPointer;
                
            return _delegate;
        }

        public static implicit operator bool(FunctionPtr<TDelegate> value) => value.GetFunctionAddress(0) != IntPtr.Zero;
    }
}