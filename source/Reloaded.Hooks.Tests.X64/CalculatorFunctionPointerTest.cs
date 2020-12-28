using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Tests.Shared;
using Xunit;
using FuncPtr = Reloaded.Hooks.Definitions.Structs.CdeclFuncPtr<int,int,int>;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorFunctionPointerTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private static IHook<FuncPtr> _addHook;
        private static IHook<FuncPtr> _subHook;
        private static IHook<FuncPtr> _multiplyHook;
        private static IHook<FuncPtr> _divideHook;

        private FuncPtr _addFunctionPointer;
        private FuncPtr _subFunctionPointer;
        private FuncPtr _multiplyFunctionPointer;
        private FuncPtr _divideFunctionPointer;

        public unsafe CalculatorFunctionPointerTest()
        {
            _nativeCalculator = new NativeCalculator();
            // We can assign directly because convention matches.
            // The pointers are CDECL and the type is CdeclFuncPtr
            _addFunctionPointer = ReloadedHooks.Instance.CreateWrapper<FuncPtr>((long)_nativeCalculator.Add);
            _subFunctionPointer = ReloadedHooks.Instance.CreateWrapper<FuncPtr>((long)_nativeCalculator.Subtract);
            _multiplyFunctionPointer = ReloadedHooks.Instance.CreateWrapper<FuncPtr>((long)_nativeCalculator.Multiply);
            _divideFunctionPointer = ReloadedHooks.Instance.CreateWrapper<FuncPtr>((long)_nativeCalculator.Divide);
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
            static int AddHookFunction(int a, int b) => _addHook.OriginalFunction.Invoke(a, b) + 1;

            _addHook = ReloadedHooks.Instance.CreateHook<FuncPtr>((delegate*unmanaged[Stdcall]<int, int, int>)&AddHookFunction, (long)_nativeCalculator.Add).Activate();

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
        public unsafe void TestHookSub()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int SubHookFunction(int a, int b) => _subHook.OriginalFunction.Invoke(a, b) + 1;

            _subHook = ReloadedHooks.Instance.CreateHook<FuncPtr>((delegate*unmanaged[Stdcall]<int, int, int>)&SubHookFunction, (long)_nativeCalculator.Subtract).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x - y) + 1;
                    int result = _subFunctionPointer.Invoke(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public unsafe void TestHookMul()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int MulHookfunction(int a, int b) => _multiplyHook.OriginalFunction.Invoke(a, b) * 2;

            _multiplyHook = ReloadedHooks.Instance.CreateHook<FuncPtr>((delegate*unmanaged[Stdcall]<int, int, int>)&MulHookfunction, (long)_nativeCalculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunctionPointer.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public unsafe void TestHookDiv()
        {
            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int DivHookfunction(int a, int b) => _divideHook.OriginalFunction.Invoke(a, b) * 2;

            _divideHook = ReloadedHooks.Instance.CreateHook<FuncPtr>((delegate*unmanaged[Stdcall]<int, int, int>)&DivHookfunction, (long)_nativeCalculator.Divide).Activate();

            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = (x / y) * 2;
                int result = _divideFunctionPointer.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }
#endif
    }
}
