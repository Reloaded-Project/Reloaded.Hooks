using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reloaded.Hooks.Definitions
{
    /// <summary>
    /// Defines common options which can be used for function hooks.
    /// </summary>
    public class FunctionHookOptions : Attribute
    {
        /// <summary>
        /// Tries to use a relative jump e.g. "jmp 0x123456" instead of absolute (pointer based)
        /// jump "jmp [0x123456]" when possible at the beginning of the hooked function. This is sometimes
        /// useful when patching over a function and you believe the built in function patcher
        /// is not picking up the return address.
        /// </summary>
        public bool PreferRelativeJump { get; set; } = false;

        /// <summary>
        /// [Related To: Function Patcher]
        /// If true, when patching return addresses for hooks, searches other modules' (DLLs) code
        /// for the original bytes to patch.
        ///
        /// [Note: Enabling this hurts performance and is very rarely needed.]
        /// </summary>
        public bool SearchInModules { get; set; } = false;

        /// <summary>
        /// [Related To: Function Patcher] 
        /// If true, check if existing hook jumps in the function prologue don't point to a valid module.
        /// If they point to a module, this indicates the jmp was probably part of the original program code and is not a hook.
        ///
        /// [Note: Enabling this hurts performance and is very rarely needed.]
        /// </summary>
        public bool VerifyJumpTargetsModule { get; set; } = false;
    }
}
