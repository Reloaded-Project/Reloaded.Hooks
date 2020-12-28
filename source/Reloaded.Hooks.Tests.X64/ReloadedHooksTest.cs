using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;
using FuncPtr = Reloaded.Hooks.Definitions.Structs.StdcallFuncPtr<int, int, int>;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class ReloadedHooksTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;

        private static IHook<NativeCalculator.AddFunction> _addHook;
        private static IHook<NativeCalculator.SubtractFunction> _subHook;
        private static IHook<NativeCalculator.DivideFunction> _divideHook;
        private static IHook<NativeCalculator.MultiplyFunction> _multiplyHook;

        private IFunction<NativeCalculator.AddFunction> _addFunction;
        private IFunction<NativeCalculator.SubtractFunction> _subtractFunction;
        private IFunction<NativeCalculator.DivideFunction> _divideFunction;
        private IFunction<NativeCalculator.MultiplyFunction> _multiplyFunction;

        private IReloadedHooks _hooks;

        public ReloadedHooksTest()
        {
            _nativeCalculator = new NativeCalculator();
            _hooks = new ReloadedHooks();

            _addFunction = _hooks.CreateFunction<NativeCalculator.AddFunction>((long)_nativeCalculator.Add);
            _subtractFunction = _hooks.CreateFunction<NativeCalculator.SubtractFunction>((long)_nativeCalculator.Subtract);
            _divideFunction = _hooks.CreateFunction<NativeCalculator.DivideFunction>((long)_nativeCalculator.Divide);
            _multiplyFunction = _hooks.CreateFunction<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply);
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
    }
}
