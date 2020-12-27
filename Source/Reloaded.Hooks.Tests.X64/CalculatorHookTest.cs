using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorHookTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private NativeCalculator.AddFunction _addFunction;
        private NativeCalculator.SubtractFunction _subtractFunction;
        private NativeCalculator.DivideFunction _divideFunction;
        private NativeCalculator.MultiplyFunction _multiplyFunction;
        private NativeCalculator.AddFunction _addWithBranchFunction;

        private static IHook<NativeCalculator.AddFunction> _addHook;
        private static IHook<NativeCalculator.SubtractFunction> _subHook;
        private static IHook<NativeCalculator.DivideFunction> _divideHook;
        private static IHook<NativeCalculator.MultiplyFunction> _multiplyHook;
        private static IHook<NativeCalculator.AddFunction> _addWithBranchHook;

        public unsafe CalculatorHookTest()
        {
            _nativeCalculator = new NativeCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long) _nativeCalculator.Add, out var addWrapper);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.SubtractFunction>((long)_nativeCalculator.Subtract, out _);
            _divideFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.DivideFunction>((long)_nativeCalculator.Divide, out var divWrapper);
            _multiplyFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply, out var mulWrapper);
            _addWithBranchFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator.AddWithBranch, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestHookAdd()
        {
            int Hookfunction(int a, int b) { return _addHook.OriginalFunction(a, b) + 1; }
            _addHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction, (long) _nativeCalculator.Add).Activate();
            
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
            _addWithBranchHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction, (long)_nativeCalculator.AddWithBranch).Activate();

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
            _subHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.SubtractFunction>(Hookfunction, (long)_nativeCalculator.Subtract).Activate();

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
            _multiplyHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction>(Hookfunction, (long)_nativeCalculator.Multiply).Activate();

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
            _divideHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.DivideFunction>(Hookfunction, (long)_nativeCalculator.Divide).Activate();

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
