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
        public static IHook<CalculatorFunction> _functionPointerNoCallHook;
        public static IHook<CalculatorFunction> _functionPointerDangerousHook;
        public static IHook<CalculatorFunctionManaged> _functionPointerNoCallManagedHook;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static int FunctionPointerHookFunction(int a, int b) => NativeCalculator._functionPointerHook.OriginalFunction.Value.Invoke(a, b);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static int FunctionPointerDangerousHookFunction(int a, int b) => NativeCalculator._functionPointerDangerousHook.OriginalFunction.Value.InvokeDangerous(a, b);

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        public static int FunctionPointerNoCallOriginalFunction(int a, int b) => a + b;

        /// <summary>
        /// Hey there! Are you looking at this?
        /// I'm just going to let you know that this is probably a very, very bad idea, okay?
        /// It probably messes up the GC's tracking. This is just a crazy science experiment.
        /// </summary>
        public static int FunctionPointerNoCallManaged(int a, int b) => a + b;

        /* Shared Stuff*/

        /// <summary> Performs A + B</summary>
        [Function(Definitions.X86.CallingConventions.Stdcall)]
        [Definitions.X64.Function(CallingConventions.Microsoft)]
        public delegate int AddFunction(int a, int b);

        [Function(Definitions.X86.CallingConventions.Stdcall)]
        [Definitions.X64.Function(CallingConventions.Microsoft)]
        public struct CalculatorFunction { public FuncPtr<int, int, int> Value; }

        [ManagedFunction(Definitions.X86.CallingConventions.ClrCall)]
        [Function(Definitions.X86.CallingConventions.Stdcall)]
        [Definitions.X64.Function(CallingConventions.Microsoft)]
        public struct CalculatorFunctionManaged { public FuncPtr<int, int, int> Value; }

        public virtual nuint Add { get; protected set; }

    }
}