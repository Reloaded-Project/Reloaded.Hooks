using System;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private NativeCalculator.AddFunction _addFunction;
        private NativeCalculator.SubtractFunction _subtractFunction;
        private NativeCalculator.DivideFunction _divideFunction;
        private NativeCalculator.MultiplyFunction _multiplyFunction;

        public CalculatorTest()
        {
            _nativeCalculator = new NativeCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long) _nativeCalculator.Add, out _);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.SubtractFunction>((long)_nativeCalculator.Subtract, out _);
            _divideFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.DivideFunction>((long)_nativeCalculator.Divide, out _);
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestAdd()
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = x + y;
                    int result   = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestSub()
        {
            int x = 100;
            for (int y = 100; y >= 0; y--)
            {
                int expected = x - y;
                int result = _subtractFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestMul()
        {
            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = x * y;
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestDiv()
        {
            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = x / y;
                int result = _divideFunction(x, y);

                Assert.Equal(expected, result);
            }
        }
    }
}
