using System;
using System.Diagnostics.CodeAnalysis;
using Reloaded.Hooks.Definitions.Helpers;

namespace Reloaded.Hooks.Definitions
{
    /// <summary/>
    public interface IReverseWrapper
    {
        /// <summary> Pointer to the function that gets executed inside the wrapper, either native or C#. </summary>
        IntPtr NativeFunctionPtr { get; }

        /// <summary> A pointer to our wrapper, which calls the internal method as if it were to be of another convention. </summary>
        IntPtr WrapperPointer { get; }
    }

    /// <summary/>
    public interface IReverseWrapper<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
    TFunction> : IReverseWrapper
    {
        /// <summary> Copy of C# function behind the pointer. </summary>
        TFunction CSharpFunction { get; }

        // Backwards Compatibility with 2.X.X
        // DO NOT TOUCH
        #region Backwards Compatibility
        /// <inheritdoc cref="IReverseWrapper.NativeFunctionPtr"/>>
        new IntPtr NativeFunctionPtr { get; }

        /// <inheritdoc cref="IReverseWrapper.WrapperPointer"/>>
        new IntPtr WrapperPointer { get; }
        #endregion
    }
}