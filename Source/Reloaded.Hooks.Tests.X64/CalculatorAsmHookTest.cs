using System;
using System.Threading;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Enums;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.X64; // Watch out!
using Xunit;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;

namespace Reloaded.Hooks.Tests.X64
{
    public class CalculatorAsmHookTest : IDisposable
    {
        private Calculator _calculator;
        private Calculator.AddFunction _addFunction;
        private Calculator.SubtractFunction _subtractFunction;
        private Calculator.AddFunction _addWithBranchFunction;

        private IAsmHook _addNoOriginalHook;
        private IAsmHook _addBeforeOriginalHook;
        private IAsmHook _addAfterOriginalHook;
        private IAsmHook _addWithBranchHook;

        public CalculatorAsmHookTest()
        {
            _calculator = new Calculator();
            _addFunction = Wrapper.Create<Calculator.AddFunction>((long) _calculator.Add);
            _subtractFunction = Wrapper.Create<Calculator.SubtractFunction>((long)_calculator.Subtract);
            _addWithBranchFunction = Wrapper.Create<Calculator.AddFunction>((long)_calculator.AddWithBranch);
        }

        public void Dispose()
        {
            _calculator?.Dispose();
        }

        [Fact]
        public void TestHookAddNoOriginal()
        {
            int wordSize = IntPtr.Size;
            string[] addFunction = 
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
                $"add {_eax}, 1",                         // Left Parameter
            };

            _addNoOriginalHook = new AsmHook(addFunction, (long) _calculator.Add, AsmHookBehaviour.DoNotExecuteOriginal).Activate();

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
        public void TestHookAddBeforeOriginal()
        {
            int wordSize = IntPtr.Size;
            string[] addFunction =
            {
                $"{_use32}",
                $"add [{_esp} + {wordSize * 1}], byte 1",      // Left Parameter
            };

            _addBeforeOriginalHook = new AsmHook(addFunction, (long)_calculator.Add, AsmHookBehaviour.ExecuteFirst).Activate();

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
        public void TestHookAddAfterOriginal()
        {
            string[] addFunction =
            {
                $"{_use32}",
                $"add {_eax}, 1", // Left Parameter - Should have already been copied from stack.
            };

            _addAfterOriginalHook = new AsmHook(addFunction, (long)_calculator.Add, AsmHookBehaviour.ExecuteAfter).Activate();

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
                $"{_use32}",
                $"add [{_esp} + {wordSize * 1}], byte 1",      // Left Parameter
            };

            _addWithBranchHook = new AsmHook(addFunction, (long)_calculator.AddWithBranch, AsmHookBehaviour.ExecuteFirst).Activate();

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
