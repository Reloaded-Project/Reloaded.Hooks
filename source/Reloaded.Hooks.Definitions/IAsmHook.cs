using System;
using System.Collections.Generic;
using System.Text;

namespace Reloaded.Hooks.Definitions
{
    /// <summary>
    /// Represents an individual x86/x64 Cheat Engine style assembly hook.
    /// </summary>
    public interface IAsmHook
    {
        /// <summary>
        /// True if the hook is enabled, else false.
        /// </summary>
        bool IsEnabled { get; }

        /// <summary>
        /// Performs a one time activation of the hook.
        /// This function should only ever be called once.
        /// </summary>
        IAsmHook Activate();

        /// <summary>
        /// Enables the current hook if it is disabled.
        /// </summary>
        void Enable();

        /// <summary>
        /// Disables the current hook if it is enabled.
        /// </summary>
        void Disable();
    }
}
