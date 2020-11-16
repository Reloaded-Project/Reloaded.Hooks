using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Tests.Shared;
using Xunit;
using FuncPtr = Reloaded.Hooks.Definitions.Structs.StdcallFuncPtr<int,int,int>;

// Watch out!

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorFunctionPointerTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private static IHook<NativeCalculator.AddFunction, FuncPtr> _addHook;
        private static IHook<NativeCalculator.MultiplyFunction, FuncPtr> _multiplyHook;
        
        private FuncPtr _addFunctionPointer;
        private FuncPtr _multiplyFunctionPointer;

        public CalculatorFunctionPointerTest()
        {
            _nativeCalculator = new NativeCalculator();
            _addFunctionPointer = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator.Add);
            _multiplyFunctionPointer = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.MultiplyFunction>((long)_nativeCalculator.Multiply);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

#if FEATURE_UNMANAGED_CALLERS_ONLY
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static int AddHookFunction(int a, int b) => _addHook.OriginalFunction(a, b) + 1;

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static int MulHookfunction(int a, int b) => _multiplyHook.OriginalFunction(a, b) * 2;

        [Fact]
        public unsafe void TestFunctionPointerHookAdd()
        {
            _addHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction, FuncPtr>((delegate*unmanaged[Cdecl]<int, int, int>)&AddHookFunction, (long)_nativeCalculator.Add).Activate();

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
            _multiplyHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.MultiplyFunction, FuncPtr>((delegate*unmanaged[Cdecl]<int, int, int>)&MulHookfunction, (long)_nativeCalculator.Multiply).Activate();

            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = (x * y) * 2;
                int result = _multiplyFunctionPointer.Invoke(x, y);

                Assert.Equal(expected, result);
            }
        }
#endif
    }
}
