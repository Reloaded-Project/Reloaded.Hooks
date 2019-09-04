namespace Reloaded.Hooks.Enums
{
    /// <summary>
    /// Defines the behaviour used by the <see cref="AsmHook"/>
    /// </summary>
    public enum AsmHookBehaviour
    {
        /// <summary>
        /// Executes your assembly code before the original.
        /// </summary>
        ExecuteFirst,

        /// <summary>
        /// Executes your assembly code after the original.
        /// </summary>
        ExecuteAfter,

        /// <summary>
        /// Do not execute original replaced code. (Dangerous!)
        /// </summary>
        DoNotExecuteOriginal
    }
}