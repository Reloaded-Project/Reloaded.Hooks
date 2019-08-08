using System;

namespace Reloaded.Hooks.Tests.Shared.Macros
{
    public static class Macros
    {
        public static string _use32 = Environment.Is64BitProcess ? "use64" : "use32";
        public static string _eax = Environment.Is64BitProcess ? "rax" : "eax";
        public static string _ebx = Environment.Is64BitProcess ? "rbx" : "ebx";
        public static string _ecx = Environment.Is64BitProcess ? "rcx" : "ecx";
        public static string _edx = Environment.Is64BitProcess ? "rdx" : "edx";
        public static string _esi = Environment.Is64BitProcess ? "rsi" : "esi";
        public static string _edi = Environment.Is64BitProcess ? "rdi" : "edi";
        public static string _ebp = Environment.Is64BitProcess ? "rbp" : "ebp";
        public static string _esp = Environment.Is64BitProcess ? "rsp" : "esp";
    }
}
