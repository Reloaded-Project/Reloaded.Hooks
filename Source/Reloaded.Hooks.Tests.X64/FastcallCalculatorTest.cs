using System;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{

    public class FastcallCalculatorTest : IDisposable
    {
        private FastcallCalculator _calculator;
        private FastcallCalculator.AddFunction _addFunction;
        private FastcallCalculator.SubtractFunction _subtractFunction;
        private FastcallCalculator.DivideFunction _divideFunction;
        private FastcallCalculator.MultiplyFunction _multiplyFunction;

        public FastcallCalculatorTest()
        {
            _calculator = new FastcallCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.AddFunction>((long) _calculator.Add, out _);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.SubtractFunction>((long)_calculator.Subtract, out _);
            _divideFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.DivideFunction>((long)_calculator.Divide, out _);
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.MultiplyFunction>((long)_calculator.Multiply, out _);
        }

        public void Dispose()
        {
            _calculator?.Dispose();
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
