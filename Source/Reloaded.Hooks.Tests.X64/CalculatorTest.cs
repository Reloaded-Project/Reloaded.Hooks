using System;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X64; // Watch out!
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{

    public class CalculatorTest : IDisposable
    {
        private Calculator _calculator;
        private Calculator.AddFunction _addFunction;
        private Calculator.SubtractFunction _subtractFunction;
        private Calculator.DivideFunction _divideFunction;
        private Calculator.MultiplyFunction _multiplyFunction;

        public CalculatorTest()
        {
            _calculator = new Calculator();
            _addFunction = Wrapper.Create<Calculator.AddFunction>((long) _calculator.Add);
            _subtractFunction = Wrapper.Create<Calculator.SubtractFunction>((long)_calculator.Subtract);
            _divideFunction = Wrapper.Create<Calculator.DivideFunction>((long)_calculator.Divide);
            _multiplyFunction = Wrapper.Create<Calculator.MultiplyFunction>((long)_calculator.Multiply);
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
