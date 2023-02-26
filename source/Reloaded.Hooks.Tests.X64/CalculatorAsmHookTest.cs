using System;
using System.Threading;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tests.Shared.Macros;
using Xunit;

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorAsmHookTest : IDisposable
    {
        private NativeCalculator _nativeCalculator;
        private NativeCalculator.AddFunction _addFunction;
        private NativeCalculator.SubtractFunction _subtractFunction;
        private NativeCalculator.AddFunction _addWithBranchFunction;

        private IAsmHook _addNoOriginalHook;
        private IAsmHook _addBeforeOriginalHook;
        private IAsmHook _addAfterOriginalHook;
        private IAsmHook _addWithBranchHook;

        public CalculatorAsmHookTest()
        {
            _nativeCalculator = new NativeCalculator();
            _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long) _nativeCalculator.Add, out _);
            _subtractFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.SubtractFunction>((long)_nativeCalculator.Subtract, out _);
            _addWithBranchFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator.AddWithBranch, out _);
        }

        public void Dispose()
        {
            _nativeCalculator?.Dispose();
        }

        [Fact]
        public void TestHookAddNoOriginal() => TestHookAddNoOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.DoNotExecuteOriginal });

        [Fact]
        public void TestHookAddNoOriginalRelative() => TestHookAddNoOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.DoNotExecuteOriginal, PreferRelativeJump = true });

        private void TestHookAddNoOriginal_Internal(AsmHookOptions options)
        {
            int wordSize = IntPtr.Size;
            string addFunction =
@$"{Macros._use32}
push {Macros._ebp}
mov {Macros._ebp}, {Macros._esp}
mov {Macros._eax}, [{Macros._ebp} + {wordSize * 2}]
mov {Macros._ecx}, [{Macros._ebp} + {wordSize * 3}] 
add {Macros._eax}, 1";

            _addNoOriginalHook = ReloadedHooks.Instance.CreateAsmHook(addFunction, (long)_nativeCalculator.Add, options).Activate();

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

        [Fact]
        public void TestHookAddBeforeOriginal() => TestHookAddBeforeOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.ExecuteFirst });

        [Fact]
        public void TestHookAddBeforeOriginalRelative() => TestHookAddBeforeOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.ExecuteFirst, PreferRelativeJump = true });

        private void TestHookAddBeforeOriginal_Internal(AsmHookOptions options)
        {
            int wordSize = IntPtr.Size;
            string[] addFunction =
            {
                $"{Macros._use32}",
                $"add [{Macros._esp} + {wordSize * 1}], byte 1",      // Left Parameter
            };

            _addBeforeOriginalHook = ReloadedHooks.Instance.CreateAsmHook(addFunction, (long)_nativeCalculator.Add, options).Activate();

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

        [Fact]
        public void TestHookAddAfterOriginal() => TestHookAddAfterOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.ExecuteAfter });

        [Fact]
        public void TestHookAddAfterOriginalRelative() => TestHookAddAfterOriginal_Internal(new AsmHookOptions() { Behaviour = AsmHookBehaviour.ExecuteAfter, PreferRelativeJump = true });

        private void TestHookAddAfterOriginal_Internal(AsmHookOptions options)
        {
            string[] addFunction =
            {
                $"{Macros._use32}",
                $"add {Macros._eax}, 1", // Left Parameter - Should have already been copied from stack.
            };

            _addAfterOriginalHook = ReloadedHooks.Instance.CreateAsmHook(addFunction, (long)_nativeCalculator.Add, options).Activate();

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

        [Fact]
        public void TestHookAddWithBranch()
        {
            int wordSize = IntPtr.Size;
            string[] addFunction =
            {
                $"{Macros._use32}",
                $"add [{Macros._esp} + {wordSize * 1}], byte 1",      // Left Parameter
            };

            _addWithBranchHook = ReloadedHooks.Instance.CreateAsmHook(addFunction, (long)_nativeCalculator.AddWithBranch, AsmHookBehaviour.ExecuteFirst).Activate();

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
    }
}
