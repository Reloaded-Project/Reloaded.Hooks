using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Reloaded.Hooks.Definitions;

namespace Reloaded.Hooks.Benchmarks
{
    public unsafe class HookBenchmark<TNativeCalculator> where TNativeCalculator : NativeCalculator, new()
    {
        protected NativeCalculator _normalCalculator;
        protected NativeCalculator _delegateHookedCalculator;
        protected NativeCalculator _delegateHookedCalculatorNoCallOriginal;
        protected NativeCalculator _functionPtrHookedCalculator;
        protected NativeCalculator _functionPtrHookedCalculatorNoCall;
        protected NativeCalculator _functionPtrHookedCalculatorNoCallManaged;
        protected NativeCalculator _functionPtrDangerousHookedCalculator;
        
        protected NativeCalculator.AddFunction _delegateHookAddFunctionNoCallOriginal;
        protected NativeCalculator.AddFunction _delegateHookAddFunction;
        protected NativeCalculator.AddFunction _delegateAddFunction;

        protected static IHook<NativeCalculator.AddFunction> _delegateHook;
        protected static IHook<NativeCalculator.AddFunction> _delegateHookNoCallOriginal;

        protected NativeCalculator.CalculatorFunction _pointerHookDangerousAddFunction;
        protected NativeCalculator.CalculatorFunction _pointerHookNoCallAddFunction;
        protected NativeCalculator.CalculatorFunction _pointerHookNoCallManagedAddFunction;
        protected NativeCalculator.CalculatorFunction _pointerHookAddFunction;
        protected NativeCalculator.CalculatorFunction _pointerDangerousAddFunction;
        protected NativeCalculator.CalculatorFunction _pointerAddFunction;

        protected int _numIterations = 1000000;

        [GlobalSetup]
        public void Setup()
        {
            _normalCalculator = new TNativeCalculator();
            _delegateHookedCalculator = new TNativeCalculator();
            _delegateHookedCalculatorNoCallOriginal = new TNativeCalculator();
            _functionPtrHookedCalculator = new TNativeCalculator();
            _functionPtrHookedCalculatorNoCall = new TNativeCalculator();
            _functionPtrHookedCalculatorNoCallManaged = new TNativeCalculator();
            _functionPtrDangerousHookedCalculator = new TNativeCalculator();
            var hooks = ReloadedHooks.Instance;

            _delegateHookAddFunctionNoCallOriginal = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_delegateHookedCalculatorNoCallOriginal.Add, out var _);
            _delegateHookAddFunction = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_delegateHookedCalculator.Add, out var _);
            _pointerHookAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrHookedCalculator.Add, out var _);
            _pointerHookNoCallAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrHookedCalculatorNoCall.Add, out var _);
            _pointerHookNoCallManagedAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrHookedCalculatorNoCallManaged.Add, out var _);
            _pointerHookDangerousAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrDangerousHookedCalculator.Add, out var _);

            _delegateAddFunction = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_normalCalculator.Add, out var _);
            _pointerAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_normalCalculator.Add, out var _);
            _pointerDangerousAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_normalCalculator.Add, out var _);

            _delegateHookNoCallOriginal = hooks.CreateHook<NativeCalculator.AddFunction>(DelegateAddHookFunctionNoCallOriginal, (long)_delegateHookedCalculatorNoCallOriginal.Add).Activate();
            _delegateHook = hooks.CreateHook<NativeCalculator.AddFunction>(DelegateAddHookFunction, (long)_delegateHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>((delegate*unmanaged[Stdcall]<int,int,int>)& NativeCalculator.FunctionPointerHookFunction, (long)_functionPtrHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerDangerousHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>(typeof(NativeCalculator), nameof(NativeCalculator.FunctionPointerDangerousHookFunction), (long)_functionPtrDangerousHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerNoCallHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>(typeof(NativeCalculator), nameof(NativeCalculator.FunctionPointerNoCallOriginalFunction), (long)_functionPtrHookedCalculatorNoCall.Add).Activate();
            NativeCalculator._functionPointerNoCallManagedHook = hooks.CreateHook<NativeCalculator.CalculatorFunctionManaged>(typeof(NativeCalculator), nameof(NativeCalculator.FunctionPointerNoCallManaged), (long)_functionPtrHookedCalculatorNoCallManaged.Add).Activate();
        }

        private int DelegateAddHookFunction(int a, int b) => _delegateHook.OriginalFunction(a, b);
        private int DelegateAddHookFunctionNoCallOriginal(int a, int b) => a + b;

        [MethodImpl(MethodImplOptions.NoInlining)]
        static int Add(int a, int b) { return a + b; }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        static int AddUnopt(int a, int b) { return a + b; }

        [Benchmark]
        public int ManagedInlined()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += (y + y);
            }

            return value;
        }

        [Benchmark]
        public int Managed()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += Add(y, y);
            }

            return value;
        }

        [Benchmark]
        public int ManagedUnoptimized()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += AddUnopt(y, y);
            }

            return value;
        }

        [Benchmark]
        public int NoHookPointerDangerous()
        {
            int value = 0;

            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerDangerousAddFunction.Value.InvokeDangerous(y,y);
            }

            return value;
        }

        [Benchmark]
        public int NoHookPointer()
        {
            int value = 0;

            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerAddFunction.Value.Invoke(y,y);
            }

            return value;
        }

        [Benchmark]
        public int NoHookDelegate()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _delegateAddFunction(y, y);
            }

            return value;
        }

        [Benchmark]
        public int DelegateHookNoCallOriginal()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _delegateHookAddFunctionNoCallOriginal(y, y);
            }

            return value;
        }
        [Benchmark]
        public int FuncPtrHookNoCallOriginal()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerHookNoCallAddFunction.Value.Invoke(y, y);
            }

            return value;
        }

        [Benchmark]
        public int DelegateHook()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _delegateHookAddFunction(y, y);
            }

            return value;
        }

        [Benchmark]
        public int FuncPtrHook()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerHookAddFunction.Value.Invoke(y, y);
            }

            return value;
        }

        [Benchmark]
        public int FuncPtrHookDangerous()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerHookDangerousAddFunction.Value.Invoke(y, y);
            }

            return value;
        }
    }
}
