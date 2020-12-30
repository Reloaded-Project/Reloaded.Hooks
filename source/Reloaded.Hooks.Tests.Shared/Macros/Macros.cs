using System;

namespace Reloaded.Hooks.Tests.Shared.Macros
{
    public static class Macros
    {
        public static bool Is64Bit = Environment.Is64BitProcess;

        public static string _use32 = Is64Bit ? "use64" : "use32";
        public static string _eax = Is64Bit ? "rax" : "eax";
        public static string _ebx = Is64Bit ? "rbx" : "ebx";
        public static string _ecx = Is64Bit ? "rcx" : "ecx";
        public static string _edx = Is64Bit ? "rdx" : "edx";
        public static string _esi = Is64Bit ? "rsi" : "esi";
        public static string _edi = Is64Bit ? "rdi" : "edi";
        public static string _ebp = Is64Bit ? "rbp" : "ebp";
        public static string _esp = Is64Bit ? "rsp" : "esp";

        /// <summary>
        /// Represents the full word operand size for current architecture.
        /// </summary>
        public static string _word = Is64Bit ? "qword" : "dword";
    }
}
