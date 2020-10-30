using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class FastcallCalculatorHookTest : IDisposable
    {
        private FastcallCalculator _calculator;
        private FastcallCalculator.AddFunction _addFunction;
        private FastcallCalculator.SubtractFunction _subtractFunction;
        private FastcallCalculator.DivideFunction _divideFunction;
        private FastcallCalculator.MultiplyFunction _multiplyFunction;

        private static IHook<FastcallCalculator.AddFunction> _addHook;
        private static IHook<FastcallCalculator.SubtractFunction> _subHook;
        private static IHook<FastcallCalculator.DivideFunction> _divideHook;
        private static IHook<FastcallCalculator.MultiplyFunction> _multiplyHook;

        private Definitions.Structs.FuncPtr<int, int, int> _addFunctionPointer;
        private Definitions.Structs.FuncPtr<int, int, int> _multiplyFunctionPointer;

        public FastcallCalculatorHookTest()
        {
            _calculator = new FastcallCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.AddFunction>((long) _calculator.Add, out var addFunction);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.SubtractFunction>((long)_calculator.Subtract, out _);
            _divideFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.DivideFunction>((long)_calculator.Divide, out _);
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<FastcallCalculator.MultiplyFunction>((long)_calculator.Multiply, out var mulFunction);
            _addFunctionPointer = addFunction;
            _multiplyFunctionPointer = mulFunction;
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [UnmanagedCallersOnly(CallConvs = new [] { typeof(CallConvCdecl) })]
        static int AddHookfunction(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static int MulHookfunction(int a, int b) { return _multiplyHook.OriginalFunction(a, b) * 2; }

        [Fact]
        public unsafe void TestFunctionPointerHookAdd()
        {
            _addHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.AddFunction>((delegate*unmanaged[Cdecl]<int, int, int>)&AddHookfunction, (long)_calculator.Add).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunctionPointer.Invoke(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public unsafe void TestFunctionPointerHookMul()
        {
            _multiplyHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.MultiplyFunction>((delegate*unmanaged[Cdecl]<int, int, int>)&MulHookfunction, (long)_calculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunctionPointer.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestHookAdd()
        {
            int Hookfunction(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            _addHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.AddFunction>(Hookfunction, (long) _calculator.Add).Activate();
            
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
        public void TestHookSub()
        {
            int Hookfunction(int a, int b) { return _subHook.OriginalFunction(a, b) - 1; }
            _subHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.SubtractFunction>(Hookfunction, (long)_calculator.Subtract).Activate();

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
            _multiplyHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.MultiplyFunction>(Hookfunction, (long)_calculator.Multiply).Activate();

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
            _divideHook = ReloadedHooks.Instance.CreateHook<FastcallCalculator.DivideFunction>(Hookfunction, (long)_calculator.Divide).Activate();

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
