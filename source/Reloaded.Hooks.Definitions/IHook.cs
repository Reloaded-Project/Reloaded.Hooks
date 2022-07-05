using System;
using System.Diagnostics.CodeAnalysis;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Structs;

namespace Reloaded.Hooks.Definitions
{
    /// <summary/>
    public interface IHook
    {
        /// <summary>
        /// Returns true if the hook is enabled and currently functional, else false.
        /// </summary>
        bool IsHookEnabled { get; }

        /// <summary>
        /// Returns true if the hook has been activated.
        /// The hook may only be activated once.
        /// </summary>
        bool IsHookActivated { get; }

        /// <summary>
        /// The address to call if you wish to call the <see cref="IHook{TFunction}.OriginalFunction"/>.
        /// </summary>
        IntPtr OriginalFunctionAddress { get; }

        /// <summary>
        /// The address of the wrapper used to call the <see cref="IHook{TFunction}.OriginalFunction"/>.
        /// If no wrapper was generated, this value is the same as <see cref="OriginalFunctionAddress"/>
        /// </summary>
        IntPtr OriginalFunctionWrapperAddress { get; }

#if FEATURE_DEFAULT_INTERFACES
        /// <summary>
        /// Performs a one time activation of the hook, making the necessary memory writes to permanently commit the hook.
        /// </summary>
        /// <remarks>
        ///     This function should be called after instantiation as soon as possible,
        ///     preferably in the same line as instantiation.
        ///
        ///     This class exists such that we don't run into concurrency issues on
        ///     attaching to other processes whereby the following happens:
        ///
        ///     A. Original process calls a function that was just hooked.
        ///     B. Create function has not yet returned, and <see cref="IHook{TFunction}.OriginalFunction"/> is unassigned.
        ///     C. Hook tried to call <see cref="IHook{TFunction}.OriginalFunction"/>. <see cref="NullReferenceException"/>.
        /// </remarks>
        IHook Activate() => throw new NotImplementedException($"{nameof(Activate)} is not implemented. Most likely this code was built before IHook<T> split into IHook and IHook<T>.");
#else
        /// <summary>
        /// Performs a one time activation of the hook, making the necessary memory writes to permanently commit the hook.
        /// </summary>
        /// <remarks>
        ///     This function should be called after instantiation as soon as possible,
        ///     preferably in the same line as instantiation.
        ///
        ///     This class exists such that we don't run into concurrency issues on
        ///     attaching to other processes whereby the following happens:
        ///
        ///     A. Original process calls a function that was just hooked.
        ///     B. Create function has not yet returned, and <see cref="IHook{TFunction}.OriginalFunction"/> is unassigned.
        ///     C. Hook tried to call <see cref="IHook{TFunction}.OriginalFunction"/>. <see cref="NullReferenceException"/>.
        /// </remarks>
        IHook Activate(); 
#endif


#if FEATURE_DEFAULT_INTERFACES
        /// <summary>
        /// Temporarily disables the hook, causing all functions re-routed to your own function to be re-routed back to the original function instead.
        /// </summary>
        /// <remarks>This is implemented in such a fashion that the hook shall never touch C# code.</remarks>
        void Disable() => throw new NotImplementedException($"{nameof(Disable)} is not implemented. Most likely this code was built before IHook<T> split into IHook and IHook<T>.");
#else
        /// <summary>
        /// Temporarily disables the hook, causing all functions re-routed to your own function to be re-routed back to the original function instead.
        /// </summary>
        /// <remarks>This is implemented in such a fashion that the hook shall never touch C# code.</remarks>
        void Disable();
#endif

#if FEATURE_DEFAULT_INTERFACES
        /// <summary>
        /// Re-enables the hook if it has been disabled, causing all functions to be once again re-routed to your own function.
        /// </summary>
        void Enable() => throw new NotImplementedException($"{nameof(Enable)} is not implemented. Most likely this code was built before IHook<T> split into IHook and IHook<T>.");
#else
        /// <summary>
        /// Re-enables the hook if it has been disabled, causing all functions to be once again re-routed to your own function.
        /// </summary>
        void Enable();
#endif
    }

    /// <summary/>
    /// <typeparam name="TFunction">A valid delegate type or struct representing a function pointer.</typeparam>
    public interface IHook<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
    TFunction> : IHook
    {
        /// <summary>
        /// Allows you to call the original function that was hooked.
        /// </summary>
        TFunction OriginalFunction { get; }

        /// <summary>
        /// The reverse function wrapper that allows us to call the C# function
        /// as if it were to be of another calling convention.
        /// </summary>
        IReverseWrapper<TFunction> ReverseWrapper { get; }

        // Backwards compatibility elements for 2.X.X.
        // DO NOT TOUCH
#region Backwards Compatibility
        /// <inheritdoc cref="IHook.IsHookEnabled"/>>
        new bool IsHookEnabled { get; }

        /// <inheritdoc cref="IHook.IsHookActivated"/>>
        new bool IsHookActivated { get; }

        /// <inheritdoc cref="IHook.OriginalFunctionAddress"/>>
        new IntPtr OriginalFunctionAddress { get; }

        /// <inheritdoc cref="IHook.OriginalFunctionWrapperAddress"/>>
        new IntPtr OriginalFunctionWrapperAddress { get; }

        /// <inheritdoc cref="IHook.Activate"/>>
        new IHook<TFunction> Activate();

        /// <inheritdoc cref="IHook.Disable"/>>
        new void Disable();

        /// <inheritdoc cref="IHook.Enable"/>>
        new void Enable();
#endregion
    }
}