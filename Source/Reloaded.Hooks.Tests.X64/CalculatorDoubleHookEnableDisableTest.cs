using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    /// <summary>
    /// Tests the hook enabling and disabling functionality when hooks are stacked.
    /// </summary>
    public class CalculatorDoubleHookEnableDisableTest : IDisposable
    {
        private const int MultiplyConstant = 2;

        private NativeCalculator _nativeCalculator;
        private NativeCalculator.MultiplyFunction _multiplyFunction;

        private IHook<NativeCalculator.MultiplyFunction>   _multiplyHook01;
        private IHook<NativeCalculator.MultiplyFunction>   _multiplyHook02;

        public CalculatorDoubleHookEnableDisableTest()
        {
            _nativeCalculator = new NativeCalculator();
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestHookMul()
        {
            int Hookfunction01(int a, int b) { return _multiplyHook01.OriginalFunction(a, b) * MultiplyConstant; }
            int Hookfunction02(int a, int b) { return _multiplyHook02.OriginalFunction(a, b) * MultiplyConstant; }

            _multiplyHook01 = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction>(Hookfunction01, (long)_nativeCalculator.Multiply).Activate();
            _multiplyHook02 = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction>(Hookfunction02, (long)_nativeCalculator.Multiply).Activate();

            // 11, 10, 01, 00 labels below represent hook enable states. First bit is _multiplyHook01, second is _multiplyHook02.

            // 01, 10: Common test.

            // 11
            AssertTwoHooksEnabled();

            // 10
            _multiplyHook01.Disable();
            _multiplyHook02.Enable();
            AssertOneHookEnabled();

            // 01
            _multiplyHook01.Enable();
            _multiplyHook02.Disable();
            AssertOneHookEnabled();

            // 00
            _multiplyHook01.Disable();
            _multiplyHook02.Disable();
            AssertZeroHooksEnabled();
        }

        private void AssertTwoHooksEnabled()
        {
            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = ((x * y) * MultiplyConstant) * MultiplyConstant;
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        private void AssertOneHookEnabled()
        {
            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = ((x * y) * MultiplyConstant);
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }

        private void AssertZeroHooksEnabled()
        {
            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = x * y;
                int result = _multiplyFunction(x, y);

                Assert.Equal(expected, result);
            }
        }
    }
}
