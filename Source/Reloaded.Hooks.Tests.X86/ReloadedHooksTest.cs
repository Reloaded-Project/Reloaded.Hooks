using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X64; // Watch out!
using Xunit;

namespace Reloaded.Hooks.Tests.X86
{
    public class ReloadedHooksTest : IDisposable
    {
        private Calculator _calculator;

        private IHook<Calculator.AddFunction> _addHook;
        private IHook<Calculator.SubtractFunction> _subHook;
        private IHook<Calculator.DivideFunction> _divideHook;
        private IHook<Calculator.MultiplyFunction> _multiplyHook;

        private Function<Calculator.AddFunction> _addFunction;
        private Function<Calculator.SubtractFunction> _subtractFunction;
        private Function<Calculator.DivideFunction> _divideFunction;
        private Function<Calculator.MultiplyFunction> _multiplyFunction;

        private IReloadedHooks _hooks;

        public ReloadedHooksTest()
        {
            _calculator = new Calculator();
            _hooks = new ReloadedHooks();

            _addFunction = new Function<Calculator.AddFunction>((long)_calculator.Add, _hooks);
            _subtractFunction = new Function<Calculator.SubtractFunction>((long)_calculator.Subtract, _hooks);
            _divideFunction = new Function<Calculator.DivideFunction>((long)_calculator.Divide, _hooks);
            _multiplyFunction = new Function<Calculator.MultiplyFunction>((long)_calculator.Multiply, _hooks);
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd()
        {
            int Hookfunction(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            _addHook = _addFunction.Hook(Hookfunction).Activate();
            
            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result   = _addFunction.GetWrapper()(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestHookSub()
        {
            int Hookfunction(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            _subHook = _subtractFunction.Hook(Hookfunction).Activate();

            int x = 100;
            for (int y = 100; y >= 0; y--)
            {
                int expected = (x - y) - 1;
                int result = _subtractFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookMul()
        {
            int Hookfunction(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }
            _multiplyHook = _multiplyFunction.Hook(Hookfunction).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookDiv()
        {
            int Hookfunction(int a, int b) { return _divideHook.OriginalFunction(a, b) * 2; }
            _divideHook = _divideFunction.Hook(Hookfunction).Activate();

            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = (x / y) * 2;
                int result = _divideFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }
    }
}
