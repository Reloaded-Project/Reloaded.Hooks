using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;
using static Reloaded.Hooks.Tests.Shared.NativeCalculator;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class ReloadedHooksTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;

        private static IHook<CalculatorFunction> _addHookPtr;
        private static IHook<AddFunction> _addHook;
        private static IHook<SubtractFunction> _subHook;
        private static IHook<DivideFunction> _divideHook;
        private static IHook<MultiplyFunction> _multiplyHook;

        private IFunction<AddFunction> _addFunction;
        private IFunction<SubtractFunction> _subtractFunction;
        private IFunction<DivideFunction> _divideFunction;
        private IFunction<MultiplyFunction> _multiplyFunction;

        private IReloadedHooks _hooks;

        public ReloadedHooksTest()
        {
            _nativeCalculator = new NativeCalculator();
            _hooks = new ReloadedHooks();

            _addFunction = _hooks.CreateFunction<AddFunction>((long)_nativeCalculator.Add);
            _subtractFunction = _hooks.CreateFunction<SubtractFunction>((long)_nativeCalculator.Subtract);
            _divideFunction = _hooks.CreateFunction<DivideFunction>((long)_nativeCalculator.Divide);
            _multiplyFunction = _hooks.CreateFunction<MultiplyFunction>((long)_nativeCalculator.Multiply);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
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

#if FEATURE_UNMANAGED_CALLERS_ONLY
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        unsafe static int HookAsfunction(int a, int b) { return _addHookPtr.OriginalFunction.Value.Invoke(a, b) + 1; }

        [Fact]
        public void TestHookAs()
        {
            _addHookPtr = _addFunction.HookAs<CalculatorFunction>(typeof(ReloadedHooksTest), nameof(HookAsfunction)).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunction.GetWrapper()(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }
#endif

    }
}
