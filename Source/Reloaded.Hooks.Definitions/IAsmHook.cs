using System;
using System.Collections.Generic;
using System.Text;

namespace Reloaded.Hooks.Definitions
{
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

        void Enable();
        void Disable();
    }
}
