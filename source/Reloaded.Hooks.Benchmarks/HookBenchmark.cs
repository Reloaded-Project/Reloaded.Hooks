using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using Reloaded.Hooks.Definitions;

namespace Reloaded.Hooks.Benchmarks
{
    public unsafe class HookBenchmark<TNativeCalculator> where TNativeCalculator : NativeCalculator, new()
    {
        private NativeCalculator _normalCalculator;
        private NativeCalculator _delegateHookedCalculator;
        private NativeCalculator _delegateHookedCalculatorNoCallOriginal;
        private NativeCalculator _functionPtrHookedCalculator;
        private NativeCalculator _functionPtrHookedCalculatorNoCallOriginal;
        private NativeCalculator _functionPtrDangerousHookedCalculator;
        
        private NativeCalculator.AddFunction _delegateHookAddFunctionNoCallOriginal;
        private NativeCalculator.AddFunction _delegateHookAddFunction;
        private NativeCalculator.AddFunction _delegateAddFunction;

        private static IHook<NativeCalculator.AddFunction> _delegateHook;
        private static IHook<NativeCalculator.AddFunction> _delegateHookNoCallOriginal;

        private NativeCalculator.CalculatorFunction _pointerHookDangerousAddFunction;
        private NativeCalculator.CalculatorFunction _pointerHookNoCallOriginalAddFunction;
        private NativeCalculator.CalculatorFunction _pointerHookAddFunction;
        private NativeCalculator.CalculatorFunction _pointerDangerousAddFunction;
        private NativeCalculator.CalculatorFunction _pointerAddFunction;

        private int _numIterations = 1000000;

        [GlobalSetup]
        public void Setup()
        {
            _normalCalculator = new TNativeCalculator();
            _delegateHookedCalculator = new TNativeCalculator();
            _delegateHookedCalculatorNoCallOriginal = new TNativeCalculator();
            _functionPtrHookedCalculator = new TNativeCalculator();
            _functionPtrHookedCalculatorNoCallOriginal = new TNativeCalculator();
            _functionPtrDangerousHookedCalculator = new TNativeCalculator();
            var hooks = ReloadedHooks.Instance;

            _delegateHookAddFunctionNoCallOriginal = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_delegateHookedCalculatorNoCallOriginal.Add, out var _);
            _delegateHookAddFunction = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_delegateHookedCalculator.Add, out var _);
            _pointerHookAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrHookedCalculator.Add, out var _);
            _pointerHookNoCallOriginalAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrHookedCalculatorNoCallOriginal.Add, out var _);
            _pointerHookDangerousAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_functionPtrDangerousHookedCalculator.Add, out var _);

            _delegateAddFunction = hooks.CreateWrapper<NativeCalculator.AddFunction>((long)_normalCalculator.Add, out var _);
            _pointerAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_normalCalculator.Add, out var _);
            _pointerDangerousAddFunction = hooks.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_normalCalculator.Add, out var _);

            _delegateHookNoCallOriginal = hooks.CreateHook<NativeCalculator.AddFunction>(DelegateAddHookFunctionNoCallOriginal, (long)_delegateHookedCalculatorNoCallOriginal.Add).Activate();
            _delegateHook = hooks.CreateHook<NativeCalculator.AddFunction>(DelegateAddHookFunction, (long)_delegateHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>((delegate*unmanaged[Stdcall]<int,int,int>)& NativeCalculator.FunctionPointerHookFunction, (long)_functionPtrHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerDangerousHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>(typeof(NativeCalculator), nameof(NativeCalculator.FunctionPointerDangerousHookFunction), (long)_functionPtrDangerousHookedCalculator.Add).Activate();
            NativeCalculator._functionPointerNoCallOriginalHook = hooks.CreateHook<NativeCalculator.CalculatorFunction>(typeof(NativeCalculator), nameof(NativeCalculator.FunctionPointerNoCallOriginalFunction), (long)_functionPtrHookedCalculatorNoCallOriginal.Add).Activate();
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
                value += _pointerHookNoCallOriginalAddFunction.Value.Invoke(y, y);
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
