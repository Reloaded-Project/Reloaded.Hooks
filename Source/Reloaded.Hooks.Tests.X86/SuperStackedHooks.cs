using System;
using System.Collections.Generic;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X86; // Watch out!
using Xunit;

namespace Reloaded.Hooks.Tests.X86
{
    public class SuperStackedHooks : IDisposable
    {
        private int HookCount = 5000;

        private Calculator _calculator;
        private Calculator.AddFunction _addFunction;

        private List<IHook<Calculator.AddFunction>> manyHooks = new List<IHook<Calculator.AddFunction>>();

        public SuperStackedHooks()
        {
            _calculator = new Calculator();
            _addFunction = Wrapper.Create<Calculator.AddFunction>((long) _calculator.Add);
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd()
        {
            for (int x = 0; x < HookCount; x++)
            {
                IHook<Calculator.AddFunction> addHook = null;
                addHook = new Hook<Calculator.AddFunction>((a, b) => addHook.OriginalFunction(a, b) + 1, (long)_calculator.Add).Activate();
                manyHooks.Add(addHook);
            }
            
            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + HookCount;
                    int result   = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }
    }
}
