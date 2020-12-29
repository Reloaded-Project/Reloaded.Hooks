using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Definitions.X86;
using CallingConventions = Reloaded.Hooks.Definitions.X64.CallingConventions;

namespace Reloaded.Hooks.Benchmarks
{
    public unsafe abstract class NativeCalculator
    {
        /* Function pointer hook. Wasn't sure where to put the non-generic stuff since UnmanagedCallersOnly can't exist in generic methods. */
        public static IHook<CalculatorFunction> _functionPointerHook;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static int FunctionPointerHookFunction(int a, int b) => NativeCalculator._functionPointerHook.OriginalFunction.Value.Invoke(a, b) + 1;

        /* Shared Stuff*/

        /// <summary> Performs A + B</summary>
        [Function(Definitions.X86.CallingConventions.Stdcall)]
        [Definitions.X64.Function(CallingConventions.Microsoft)]
        public delegate int AddFunction(int a, int b);

        [Function(Definitions.X86.CallingConventions.Stdcall)]
        [Definitions.X64.Function(CallingConventions.Microsoft)]
        public struct CalculatorFunction { public FuncPtr<int, int, int> Value; }

        public virtual IntPtr Add { get; protected set; }

    }
}