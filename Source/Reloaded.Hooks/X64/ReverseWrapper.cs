using System;
using System.Runtime.InteropServices;

namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// The <see cref="ReverseWrapper{TFunction}"/> is a marshaller which converts a Custom Convention function call
    /// to Microsoft X64 call.
    /// This means that you can call Microsoft x64 functions as if it they were Custom Convention calls.
    /// </summary>
    public class ReverseWrapper<TFunction> : IReverseWrapper<TFunction>
    {
        /// <summary> Copy of C# function behind the pointer. </summary>
        public TFunction CSharpFunction { get; }

        /// <summary> Pointer to the function that gets executed inside the wrapper, either native or C#. </summary>
        public IntPtr NativeFunctionPtr { get; }

        /// <summary> A pointer to our wrapper, which calls the internal method as if it were to be of another convention. </summary>
        public IntPtr WrapperPointer { get; private set; }

        /// <summary>
        /// Creates the <see cref="ReverseWrapper{TFunction}"/> which allows you to call
        /// a Microsoft X64 C# function, via a pointer as if it was a function of another calling convention.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it.
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">The function to create a pointer to.</param>
        public ReverseWrapper(TFunction function)
        {
            CSharpFunction = function;
            NativeFunctionPtr = Marshal.GetFunctionPointerForDelegate(function);
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr);
        }

        /// <summary>
        /// Creates the <see cref="ReverseWrapper{TFunction}"/> which allows you to call
        /// a native Microsoft X64 function, via a pointer as if it was a function of another calling convention.
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
            if (!reloadedFunctionAttribute.Equals(new FunctionAttribute(CallingConventions.Microsoft)))
                reverseFunctionWrapper.WrapperPointer = Wrapper.Create<TFunction>(functionPtr, new FunctionAttribute(CallingConventions.Microsoft), reloadedFunctionAttribute);
        }
    }
}
