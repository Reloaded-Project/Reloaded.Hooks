using System;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X86; // Watch out!
using Xunit;

namespace Reloaded.Hooks.Tests.X86
{
    public class CalculatorHookTest : IDisposable
    {
        private Calculator _calculator;
        private Calculator.AddFunction _addFunction;
        private Calculator.SubtractFunction _subtractFunction;
        private Calculator.DivideFunction _divideFunction;
        private Calculator.MultiplyFunction _multiplyFunction;
        private Calculator.AddFunction _addWithBranchFunction;

        private IHook<Calculator.AddFunction> _addHook;
        private IHook<Calculator.SubtractFunction> _subHook;
        private IHook<Calculator.DivideFunction> _divideHook;
        private IHook<Calculator.MultiplyFunction> _multiplyHook;

        private IHook<Calculator.AddFunction> _addWithBranchHook;

        public CalculatorHookTest()
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
            int Hookfunction(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            _addHook = new Hook<Calculator.AddFunction>(Hookfunction, (long) _calculator.Add).Activate();
            
            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result   = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestHookAddWithBranch()
        {
            int Hookfunction(int a, int b) { return _addWithBranchHook.OriginalFunction(a, b) + 1; }
            _addWithBranchHook = new Hook<Calculator.AddFunction>(Hookfunction, (long)_calculator.AddWithBranch).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addWithBranchFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestHookSub()
        {
            int Hookfunction(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            _subHook = new Hook<Calculator.SubtractFunction>(Hookfunction, (long)_calculator.Subtract).Activate();

            int x = 100;
            for (int y = 100; y >= 0; y--)
            {
                int expected = (x - y) - 1;
                int result = _subtractFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookMul()
        {
            int Hookfunction(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            _multiplyHook = new Hook<Calculator.MultiplyFunction>(Hookfunction, (long)_calculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookDiv()
        {
            int Hookfunction(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }
            _divideHook = new Hook<Calculator.DivideFunction>(Hookfunction, (long)_calculator.Divide).Activate();

            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = (x / y) * 2;
                int result = _divideFunction(x, y);

                Assert.Equal(expected, result);
            }
        }
    }
}
