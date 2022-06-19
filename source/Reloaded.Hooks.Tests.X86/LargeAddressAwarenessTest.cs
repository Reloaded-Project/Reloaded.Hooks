using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tests.Shared.Macros;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Buffers;
using Xunit;
using static Reloaded.Memory.Buffers.Internal.Kernel32.Kernel32;

namespace Reloaded.Hooks.Tests.X86
{
    public class LargeAddressAwarenessTest
    {
        /*
         * To test this, you will need to patch the EXE of the test runner. Most of them aren't large address aware.
         */

        private NativeCalculator _highMemCalculator = new NativeCalculator(new HighMemoryAllocator());
        private static IHook<NativeCalculator.AddFunction> _addDelegateHook;
        private static IHook<NativeCalculator.CalculatorFunction> _addFnPtrHook;
        private NativeCalculator.CalculatorFunction _addFunctionPointer;
        private NativeCalculator.AddFunction _addFunction;

        public LargeAddressAwarenessTest()
        {
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_highMemCalculator.Add, out _);
            _addFunctionPointer = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.CalculatorFunction>((long)_highMemCalculator.Add, out var _);
        }

        /// <summary>
        /// Attempts to create a set of <see cref="MemoryBuffer"/>s at the beginning and end of the
        /// address space, and then find the given buffers.
        /// </summary>
        [Fact(Skip = "This test needs to be ran manually with a patched test runner.")]
        public void AsmHook_SupportsLongJump()
        {
            AssertLargeAddressAware();

            int wordSize = IntPtr.Size;
            string[] addFunction =
            {
                $"{Macros._use32}",
                $"push {Macros._ebp}",
                $"mov {Macros._ebp}, {Macros._esp}",

                $"mov {Macros._eax}, [{Macros._ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {Macros._ecx}, [{Macros._ebp} + {wordSize * 3}]", // Right Parameter
                $"add {Macros._eax}, 1",                         // Left Parameter
            };

            var addNoOriginalHook = ReloadedHooks.Instance.CreateAsmHook(addFunction, (long)_highMemCalculator.Add, AsmHookBehaviour.DoNotExecuteOriginal).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        /* We just hope our code gets allocated far away in low mem! */
        [Fact(Skip = "This test needs to be ran manually with a patched test runner.")]
        public void RegularHook_SupportsLongJump_Relative() => RegularHook_SupportsLongJump_Internal(new FunctionHookOptions() { PreferRelativeJump = true });

        [Fact(Skip = "This test needs to be ran manually with a patched test runner.")]
        public void RegularHook_SupportsLongJump_Absolute() => RegularHook_SupportsLongJump_Internal(new FunctionHookOptions() { PreferRelativeJump = false });

        private void RegularHook_SupportsLongJump_Internal(FunctionHookOptions options)
        {
            AssertLargeAddressAware();

            int Hookfunction(int a, int b) { return _addDelegateHook.OriginalFunction(a, b) + 1; }
            _addDelegateHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction, (long)_highMemCalculator.Add, -1, options).Activate();

            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = (x + y) + 1;
                    int result = _addFunction(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        /* We just hope our code gets allocated far away in low mem! */
#if FEATURE_UNMANAGED_CALLERS_ONLY
        [Fact(Skip = "This test needs to be ran manually with a patched test runner.")]
        public void FunctionPointerHook_SupportsLongJump_Absolute()
        {
            FunctionPointerHook_SupportsLongJump_Internal(new FunctionHookOptions() { PreferRelativeJump = false });
        }

        [Fact(Skip = "This test needs to be ran manually with a patched test runner.")]
        public void FunctionPointerHook_SupportsLongJump_Relative()
        {
            FunctionPointerHook_SupportsLongJump_Internal(new FunctionHookOptions() { PreferRelativeJump = true });
        }

        internal unsafe void FunctionPointerHook_SupportsLongJump_Internal(FunctionHookOptions options)
        {
            AssertLargeAddressAware();

            [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
            static int AddHookFunction(int a, int b) => _addFnPtrHook.OriginalFunction.Value.Invoke(a, b) + 1;

            _addFnPtrHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.CalculatorFunction>((delegate* unmanaged[Stdcall]<int, int, int>)&AddHookFunction, (long)_highMemCalculator.Add, -1, options).Activate();

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
#endif

        void AssertLargeAddressAware()
        {
            var maxAddress = MemoryAllocatorHelpers.GetMaxAddress(true);
            if ((long)maxAddress <= int.MaxValue)
                Assert.False(true, "Test host is not large address aware!!");
        }
    }

    public class HighMemoryAllocator : IMemoryAllocator
    {
        private nuint _minAddress;
        private nuint _maxAddress;
        private MemoryBufferHelper _helper = new(Process.GetCurrentProcess());

        public HighMemoryAllocator()
        {
            _maxAddress = MemoryAllocatorHelpers.GetMaxAddress(true);
            _minAddress = (_maxAddress - 0x1F00000); // ~32 MB. hopefully OS wouldn't complain.
        }

        public nuint Allocate(int size)
        {
            var buf = Utilities.FindOrCreateBufferInRange(size, _minAddress, _maxAddress);
            return buf.Add(new byte[size]);
        }
    }
}
