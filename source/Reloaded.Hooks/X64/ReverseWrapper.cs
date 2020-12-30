using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Internal;

namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// Allows for the creation of functions with a custom calling convention which internally call functions using the conventions specified by <typeparamref name="TFunction"/>.
    /// </summary>
    public class ReverseWrapper<TFunction> : IReverseWrapper<TFunction>
    {
        /// <inheritdoc />
        public TFunction CSharpFunction { get; }

        /// <inheritdoc />
        public IntPtr NativeFunctionPtr { get; }

        /// <inheritdoc />
        public IntPtr WrapperPointer { get; private set; }

        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it (if <typeparamref name="TFunction"/> is a delegate type).
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">The function to create a pointer to.</param>
        public ReverseWrapper(TFunction function)
        {
            CSharpFunction = function;

            if (typeof(TFunction).IsValueType && !typeof(TFunction).IsPrimitive)
                NativeFunctionPtr = Unsafe.As<TFunction, IntPtr>(ref function);
            else
                NativeFunctionPtr = Marshal.GetFunctionPointerForDelegate(function);

            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr);
        }

        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <param name="function">Pointer of native function to wrap.</param>
        public ReverseWrapper(IntPtr function)
        {
            NativeFunctionPtr = function;
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr);
        }
        
        private static void Create(ReverseWrapper<TFunction> reverseFunctionWrapper, IntPtr functionPtr)
        {
            var reloadedFunctionAttribute = FunctionAttribute.GetAttribute<TFunction>();

            // Microsoft X64 is hot path, as our TFunction will already be Microsoft X64, we marshal if it's anything else.
            if (!reloadedFunctionAttribute.Equals(FunctionAttribute.Microsoft))
                reverseFunctionWrapper.WrapperPointer = Wrapper.Create<TFunction>(functionPtr, FunctionAttribute.Microsoft, reloadedFunctionAttribute);
        }
    }
}
