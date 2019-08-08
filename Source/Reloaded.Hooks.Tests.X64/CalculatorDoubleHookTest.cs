using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X64; // Watch out!
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorDoubleHookTest : IDisposable
    {
        private Calculator _calculator;
        private Calculator.AddFunction _addFunction;
        private Calculator.SubtractFunction _subtractFunction;
        private Calculator.DivideFunction _divideFunction;
        private Calculator.MultiplyFunction _multiplyFunction;
        private Calculator.AddFunction _addWithBranchFunction;

        private IHook<Calculator.AddFunction>        _addHook01;
        private IHook<Calculator.SubtractFunction>   _subHook01;
        private IHook<Calculator.DivideFunction>     _divideHook01;
        private IHook<Calculator.MultiplyFunction>   _multiplyHook01;

        private IHook<Calculator.AddFunction>        _addHook02;
        private IHook<Calculator.SubtractFunction>   _subHook02;
        private IHook<Calculator.DivideFunction>     _divideHook02;
        private IHook<Calculator.MultiplyFunction>   _multiplyHook02;

        private IHook<Calculator.AddFunction>       _addWithBranchHook01;
        private IHook<Calculator.AddFunction>       _addWithBranchHook02;

        public CalculatorDoubleHookTest()
        {
            _calculator = new Calculator();
            _addFunction = Wrapper.Create<Calculator.AddFunction>((long) _calculator.Add);
            _subtractFunction = Wrapper.Create<Calculator.SubtractFunction>((long)_calculator.Subtract);
            _divideFunction = Wrapper.Create<Calculator.DivideFunction>((long)_calculator.Divide);
            _multiplyFunction = Wrapper.Create<Calculator.MultiplyFunction>((long)_calculator.Multiply);
            _addWithBranchFunction = Wrapper.Create<Calculator.AddFunction>((long)_calculator.AddWithBranch);
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd()
        {
            int Hookfunction01(int a, int b) { return _addHook01.OriginalFunction(a, b) + 1; }
            int Hookfunction02(int a, int b) { return _addHook02.OriginalFunction(a, b) + 1; }

            _addHook01 = new Hook<Calculator.AddFunction>(Hookfunction01, (long)_calculator.Add).Activate();
            _addHook02 = new Hook<Calculator.AddFunction>(Hookfunction02, (long)_calculator.Add).Activate();

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

            _addWithBranchHook01 = new Hook<Calculator.AddFunction>(Hookfunction01, (long)_calculator.AddWithBranch).Activate();
            _addWithBranchHook02 = new Hook<Calculator.AddFunction>(Hookfunction02, (long)_calculator.AddWithBranch).Activate();

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
            _subHook01 = new Hook<Calculator.SubtractFunction>(Hookfunction01, (long)_calculator.Subtract).Activate();
            _subHook02 = new Hook<Calculator.SubtractFunction>(Hookfunction02, (long)_calculator.Subtract).Activate();

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
            _multiplyHook01 = new Hook<Calculator.MultiplyFunction>(Hookfunction01, (long)_calculator.Multiply).Activate();
            _multiplyHook02 = new Hook<Calculator.MultiplyFunction>(Hookfunction02, (long)_calculator.Multiply).Activate();

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
            _divideHook01 = new Hook<Calculator.DivideFunction>(Hookfunction01, (long)_calculator.Divide).Activate();
            _divideHook02 = new Hook<Calculator.DivideFunction>(Hookfunction02, (long)_calculator.Divide).Activate();

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
