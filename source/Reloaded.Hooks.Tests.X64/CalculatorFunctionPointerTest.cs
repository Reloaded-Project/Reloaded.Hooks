using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Tests.Shared;
using Xunit;
using CalculatorFunction = Reloaded.Hooks.Tests.Shared.NativeCalculator.CalculatorFunction;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorFunctionPointerTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;

        private static IHook<CalculatorFunction> _addHook;
        private static IHook<CalculatorFunction> _subHook;
        private static IHook<CalculatorFunction> _multiplyHook;
        private static IHook<CalculatorFunction> _divideHook;

        private CalculatorFunction _addFunctionPointer;
        private CalculatorFunction _subFunctionPointer;
        private CalculatorFunction _multiplyFunctionPointer;
        private CalculatorFunction _divideFunctionPointer;

        public unsafe CalculatorFunctionPointerTest()
        {
            _nativeCalculator = new NativeCalculator();
            // We can assign directly because convention matches.
            // The pointers are CDECL and the type is CdeclFuncPtr
            _addFunctionPointer = ReloadedHooks.Instance.CreateWrapper<CalculatorFunction>((long)_nativeCalculator.Add, out var _);
            _subFunctionPointer = ReloadedHooks.Instance.CreateWrapper<CalculatorFunction>((long)_nativeCalculator.Subtract, out var _);
            _multiplyFunctionPointer = ReloadedHooks.Instance.CreateWrapper<CalculatorFunction>((long)_nativeCalculator.Multiply, out var _);
            _divideFunctionPointer = ReloadedHooks.Instance.CreateWrapper<CalculatorFunction>((long)_nativeCalculator.Divide, out var _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

#if FEATURE_UNMANAGED_CALLERS_ONLY
        [Fact]
        public unsafe void TestHookAdd()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int AddHookFunction(int a, int b) => _addHook.OriginalFunction.Value.Invoke(a, b) + 1;

            _addHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>((delegate*unmanaged[Stdcall]<int, int, int>)&AddHookFunction, (long)_nativeCalculator.Add).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunctionPointer.Value.Invoke(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
        static unsafe int AddHookFunctionReflection(int a, int b) => _addHook.OriginalFunction.Value.Invoke(a, b) + 1;

        [Fact]
        public unsafe void TestHookAddViaReflection()
        {
            _addHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>(typeof(CalculatorFunctionPointerTest), nameof(AddHookFunctionReflection), (long)_nativeCalculator.Add).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunctionPointer.Value.Invoke(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public unsafe void TestHookSub()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int SubHookFunction(int a, int b) => _subHook.OriginalFunction.Value.Invoke(a, b) + 1;

            _subHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>((delegate*unmanaged[Stdcall]<int, int, int>)&SubHookFunction, (long)_nativeCalculator.Subtract).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x - y) + 1;
                    int result = _subFunctionPointer.Value.Invoke(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public unsafe void TestHookMul()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int MulHookfunction(int a, int b) => _multiplyHook.OriginalFunction.Value.Invoke(a, b) * 2;

            _multiplyHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>((delegate*unmanaged[Stdcall]<int, int, int>)&MulHookfunction, (long)_nativeCalculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunctionPointer.Value.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public unsafe void TestHookDiv()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int DivHookfunction(int a, int b) => _divideHook.OriginalFunction.Value.Invoke(a, b) * 2;

            _divideHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>((delegate*unmanaged[Stdcall]<int, int, int>)&DivHookfunction, (long)_nativeCalculator.Divide).Activate();

            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = (x / y) * 2;
                int result = _divideFunctionPointer.Value.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }
#endif
    }
}
