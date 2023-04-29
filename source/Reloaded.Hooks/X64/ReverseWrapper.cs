using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;

namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// Allows for the creation of functions with a custom calling convention which internally call functions using the conventions specified by <typeparamref name="TFunction"/>.
    /// </summary>
    public class ReverseWrapper<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
    TFunction> : IReverseWrapper<TFunction>
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
        public ReverseWrapper(TFunction function) : this(function, new WrapperOptions()) { }
        
        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it (if <typeparamref name="TFunction"/> is a delegate type).
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">The function to create a pointer to.</param>
        /// <param name="options">Options for the reverse wrapper made.</param>
        public ReverseWrapper(TFunction function, WrapperOptions options)
        {
            CSharpFunction = function;

            if (typeof(TFunction).IsValueType && !typeof(TFunction).IsPrimitive)
                NativeFunctionPtr = Unsafe.As<TFunction, IntPtr>(ref function);
            else
                NativeFunctionPtr = Marshal.GetFunctionPointerForDelegate(function);

            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr.ToUnsigned(), options);
        }
        
        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <param name="function">Pointer of native function to wrap.</param>
        public ReverseWrapper(nuint function) : this(function, new WrapperOptions()) { }

        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <param name="function">Pointer of native function to wrap.</param>
        /// <param name="options">Options for the reverse wrapper made.</param>
        public ReverseWrapper(nuint function, WrapperOptions options)
        {
            NativeFunctionPtr = function.ToSigned();
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, function, options);
        }
        
        private static void Create(ReverseWrapper<TFunction> reverseFunctionWrapper, nuint functionPtr, WrapperOptions wrapperOptions)
        {
            var reloadedFunctionAttribute = FunctionAttribute.GetAttribute<TFunction>();

            // Microsoft X64 is hot path, as our TFunction will already be Microsoft X64, we marshal if it's anything else.
            if (!reloadedFunctionAttribute.Equals(FunctionAttribute.Microsoft))
                reverseFunctionWrapper.WrapperPointer = Wrapper.Create<TFunction>(functionPtr, FunctionAttribute.Microsoft, reloadedFunctionAttribute, wrapperOptions).ToSigned();
            else
                reverseFunctionWrapper.WrapperPointer = Utilities.CreateJump(functionPtr, true, Constants.MaxAbsJmpSize).ToSigned();
        }
    }
}
