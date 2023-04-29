using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Internal;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Tools;

namespace Reloaded.Hooks.X86
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
        /// <inheritdoc/>
        public TFunction CSharpFunction { get; }

        /// <inheritdoc/>
        public IntPtr NativeFunctionPtr { get; }

        /// <inheritdoc/>
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
        /// <param name="options">Options used for wrapper generation.</param>
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
        /// <param name="options">Options used for wrapper generation.</param>
        public ReverseWrapper(nuint function, WrapperOptions options)
        {
            NativeFunctionPtr = function.ToSigned();
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, function, options);
        }
        
        private static void Create(ReverseWrapper<TFunction> reverseFunctionWrapper, nuint functionPtr, WrapperOptions options)
        {
            var attribute = FunctionAttribute.GetAttribute<TFunction>();

            // Hot path: Don't create wrapper if both conventions are already compatible.
            var managedFuncAttribute = Misc.TryGetAttributeOrDefault<TFunction, ManagedFunctionAttribute>();
            if (managedFuncAttribute != null)
            {
                if (managedFuncAttribute.Equals(attribute))
                {
                    reverseFunctionWrapper.WrapperPointer = Utilities.CreateJump(functionPtr, false, Constants.MaxAbsJmpSize).ToSigned();
                    return;
                }
                
                reverseFunctionWrapper.WrapperPointer = Wrapper.Create<TFunction>(functionPtr, managedFuncAttribute, attribute).ToSigned();
                return;
            }
            
            var funcPtrAttribute = Misc.TryGetAttributeOrDefault<TFunction, UnmanagedFunctionPointerAttribute>();
            if (!attribute.IsEquivalent(funcPtrAttribute))
                reverseFunctionWrapper.WrapperPointer = Wrapper.Create<TFunction>(functionPtr, attribute.GetEquivalent(funcPtrAttribute), attribute).ToSigned();
            else
                reverseFunctionWrapper.WrapperPointer = Utilities.CreateJump(functionPtr, false, Constants.MaxAbsJmpSize).ToSigned();
        }
    }
}
