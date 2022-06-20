namespace Reloaded.Hooks
{
    internal class Constants
    {
        /// <summary>
        /// Maximum size of a jmp instruction. [Mnemonic: jmp qword [qword 0xFFFFFFFF]]
        /// </summary>
        internal const int MaxAbsJmpSize = 8;
    }
}
