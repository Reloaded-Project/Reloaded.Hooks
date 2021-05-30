using BenchmarkDotNet.Attributes;

namespace Reloaded.Hooks.Benchmarks
{
    public unsafe class HookBenchmark32<T> : HookBenchmark<T> where T : NativeCalculator, new()
    {
        [Benchmark]
        public int FuncPtrHookNoCallOriginalManaged()
        {
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerHookNoCallManagedAddFunction.Value.Invoke(y, y);
            }

            return value;
        }

        [Benchmark]
        public int FuncPtrHookNoCallOriginalManagedDangerous()
        {
            // Note: This works because the new managed stack isn't set up in the target function; hence
            // it's not recognized as managed.
            int value = 0;
            for (int y = 0; y < _numIterations; y++)
            {
                value += _pointerHookNoCallManagedAddFunction.Value.InvokeDangerous(y, y);
            }

            return value;
        }
    }
}
