using System;
using System.Collections.Generic;

namespace Reloaded.Hooks.Tools
{
    /// <summary>
    /// Represents a native function pointer.
    /// </summary>
    public unsafe class FunctionPtr<TDelegate> where TDelegate : Delegate
    {
        /// <summary>
        /// The address of the pointer in memory with which this class was instantiated with.
        /// </summary>
        public ulong FunctionPointer { get; }
        
        /// <summary> Cache of already created function wrappers for functions at address. </summary>
        private readonly Dictionary<IntPtr, TDelegate> _methodCache;

        /// <summary> Caches the last used delegate. </summary>
        private TDelegate _delegate;

        /// <summary> Contains the address of the last called function. </summary>
        private IntPtr _lastFunctionPointer;

        /// <summary>
        /// Returns a delegate instance for a function at a specified index of the pointer array.
        /// Only use this if all functions in VTable use same delegate instance.
        /// </summary>
        /// <param name="index">Array index of pointer to function.</param>
        public TDelegate this[int index] => GetDelegate(index);

        /// <summary>
        /// Abstracts a pointer to a native function.
        /// </summary>
        public FunctionPtr(ulong functionPointer)
        {
            FunctionPointer = functionPointer;
            _methodCache = new Dictionary<IntPtr, TDelegate>();
        }

        /// <summary>
        /// Address of the function to which the pointer is currently pointing to.
        /// </summary>
        /// <param name="index"></param>
        public IntPtr GetFunctionAddress(int index)
        {
            IntPtr* functionPtr = (IntPtr*) FunctionPointer;
            return functionPtr[index];
        }

        /// <summary>
        /// Retrieves an delegate instance which can be used to call the function behind the function pointer.
        /// </summary>
        /// <returns>Null if the pointer is zero; else a callable delegate.</returns>
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

            if (IntPtr.Size == 4)
                _delegate = X86.Wrapper.Create<TDelegate>((long) functionPointer, out var wrapperAddress);
            else
                _delegate = X64.Wrapper.Create<TDelegate>((long)functionPointer, out var wrapperAddress);

            _methodCache[functionPointer] = _delegate;
            _lastFunctionPointer = functionPointer;
                
            return _delegate;
        }

        public static implicit operator bool(FunctionPtr<TDelegate> value) => value.GetFunctionAddress(0) != IntPtr.Zero;
    }
}