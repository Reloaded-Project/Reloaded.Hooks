using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorDoubleHookTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private NativeCalculator.AddFunction _addFunction;
        private NativeCalculator.SubtractFunction _subtractFunction;
        private NativeCalculator.DivideFunction _divideFunction;
        private NativeCalculator.MultiplyFunction _multiplyFunction;
        private NativeCalculator.AddFunction _addWithBranchFunction;

        private IHook<NativeCalculator.AddFunction>        _addHook01;
        private IHook<NativeCalculator.SubtractFunction>   _subHook01;
        private IHook<NativeCalculator.DivideFunction>     _divideHook01;
        private IHook<NativeCalculator.MultiplyFunction>   _multiplyHook01;

        private IHook<NativeCalculator.AddFunction>        _addHook02;
        private IHook<NativeCalculator.SubtractFunction>   _subHook02;
        private IHook<NativeCalculator.DivideFunction>     _divideHook02;
        private IHook<NativeCalculator.MultiplyFunction>   _multiplyHook02;

        private IHook<NativeCalculator.AddFunction> _addWithBranchHook01;
        private IHook<NativeCalculator.AddFunction> _addWithBranchHook02;

        public CalculatorDoubleHookTest()
        {
            _nativeCalculator = new NativeCalculator();
            
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long) _nativeCalculator.Add, out _);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.SubtractFunction>((long)_nativeCalculator.Subtract, out _);
            _divideFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.DivideFunction>((long)_nativeCalculator.Divide, out _);
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply, out _);
            _addWithBranchFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator.AddWithBranch, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd()
        {
            int Hookfunction01(int a, int b) { return _addHook01.OriginalFunction(a, b) + 1; }
            int Hookfunction02(int a, int b) { return _addHook02.OriginalFunction(a, b) + 1; }

            _addHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction01, (long) _nativeCalculator.Add).Activate();
            _addHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction02, (long)_nativeCalculator.Add).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = ((x + y) + 1) + 1;
                    int result   = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }


        [Fact]
        public void TestHookAddWithBranch()
        {
            int Hookfunction01(int a, int b) { return _addWithBranchHook01.OriginalFunction(a, b) + 1; }
            int Hookfunction02(int a, int b) { return _addWithBranchHook02.OriginalFunction(a, b) + 1; }

            _addWithBranchHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction01, (long)_nativeCalculator.AddWithBranch).Activate();
            _addWithBranchHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction02, (long)_nativeCalculator.AddWithBranch).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = ((x + y) + 1) + 1;
                    int result = _addWithBranchFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestHookSub()
        {
            int Hookfunction01(int a, int b) { return _subHook01.OriginalFunction(a, b) - 1; }
            int Hookfunction02(int a, int b) { return _subHook02.OriginalFunction(a, b) - 1; }
            _subHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.SubtractFunction>(Hookfunction01, (long)_nativeCalculator.Subtract).Activate();
            _subHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.SubtractFunction>(Hookfunction02, (long)_nativeCalculator.Subtract).Activate();

            int x = 100;
            for (int y = 100; y >= 0; y--)
            {
                int expected = ((x - y) - 1) - 1;
                int result = _subtractFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookMul()
        {
            int Hookfunction01(int a, int b) { return _multiplyHook01.OriginalFunction(a, b) * 2; }
            int Hookfunction02(int a, int b) { return _multiplyHook02.OriginalFunction(a, b) * 2; }
            _multiplyHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction>(Hookfunction01, (long)_nativeCalculator.Multiply).Activate();
            _multiplyHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction>(Hookfunction02, (long)_nativeCalculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = ((x * y) * 2) * 2;
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookDiv()
        {
            int Hookfunction01(int a, int b) { return _divideHook01.OriginalFunction(a, b) * 2; }
            int Hookfunction02(int a, int b) { return _divideHook02.OriginalFunction(a, b) * 2; }
            _divideHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.DivideFunction>(Hookfunction01, (long)_nativeCalculator.Divide).Activate();
            _divideHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.DivideFunction>(Hookfunction02, (long)_nativeCalculator.Divide).Activate();

            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = ((x / y) * 2) * 2;
                int result = _divideFunction(x, y);

                Assert.Equal(expected, result);
            }
        }
    }
}
