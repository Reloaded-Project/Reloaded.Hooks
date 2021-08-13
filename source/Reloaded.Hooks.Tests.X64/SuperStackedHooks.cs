using System;
using System.Collections.Generic;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class SuperStackedHooks : IDisposable
    {
        private int HookCount = 1000;

        private NativeCalculator _nativeCalculator;
        private NativeCalculator.AddFunction _addFunction;

        private List<IHook<NativeCalculator.AddFunction>> manyHooks = new List<IHook<NativeCalculator.AddFunction>>();

        public SuperStackedHooks()
        {
            _nativeCalculator = new NativeCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long) _nativeCalculator.Add, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd() => TestHookAdd_Internal(null);

        [Fact]
        public void TestHookAddRelativeJump() => TestHookAdd_Internal(new FunctionHookOptions() { PreferRelativeJump = true });

        private void TestHookAdd_Internal(FunctionHookOptions options)
        {
            for (int x = 0; x < HookCount; x++)
            {
                IHook<NativeCalculator.AddFunction> addHook = null;
                addHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>((a, b) => addHook.OriginalFunction(a, b) + 1, (long)_nativeCalculator.Add, -1, options).Activate();
                manyHooks.Add(addHook);
            }

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + HookCount;
                    int result = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }
    }
}
