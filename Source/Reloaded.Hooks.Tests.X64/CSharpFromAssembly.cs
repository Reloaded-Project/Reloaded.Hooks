using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Reloaded.Hooks.X64;
using Xunit;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;

namespace Reloaded.Hooks.Tests.X64
{
    /// <summary>
    /// Tests calling C# function from custom ASM/assembly code.
    /// </summary>
    public class CSharpFromAssembly
    {
        private Assembler.Assembler _assembler  = new Assembler.Assembler();
        private Memory.Sources.Memory _memory   = new Memory.Sources.Memory();

        private Function<ManagedCalculator.AddFunction>       _addFunction;
        private Function<ManagedCalculator.SubtractFunction>  _subtractFunction;
        private Function<ManagedCalculator.MultiplyFunction> _multiplyFunction;
        private Function<ManagedCalculator.DivideFunction>    _divideFunction;

        private IReverseWrapper<ManagedCalculator.AddFunction> _reverseWrapAddFunction;
        private IReverseWrapper<ManagedCalculator.SubtractFunction> _reverseWrapSubFunction;
        private IReverseWrapper<ManagedCalculator.MultiplyFunction> _reverseWrapMulFunction;
        private IReverseWrapper<ManagedCalculator.DivideFunction> _reverseWrapDivFunction;

        public CSharpFromAssembly()
        {
            CreateAddFunction();
            CreateSubFunction();
            CreateMulFunction();
            CreateDivFunction();
        }

        /* Tests */

        [Fact]
        public void TestAdd()
        {
            for (int x = 0; x < 100; x++)
            {
                for (int y = 1; y < 100;)
                {
                    int expected = x + y;
                    int result = _addFunction.GetWrapper()(x, y);

                    Assert.Equal(expected, result);
                    y += 2;
                }
            }
        }

        [Fact]
        public void TestSub()
        {
            int x = 100;
            for (int y = 100; y >= 0; y--)
            {
                int expected = x - y;
                int result = _subtractFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestMul()
        {
            int x = 100;
            for (int y = 0; y < 100; y++)
            {
                int expected = x * y;
                int result = _multiplyFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }

        [Fact]
        public void TestDiv()
        {
            int x = 100;
            for (int y = 1; y < 100; y++)
            {
                int expected = x / y;
                int result = _divideFunction.GetWrapper()(x, y);

                Assert.Equal(expected, result);
            }
        }

        /* Test Setup */
        private void CreateAddFunction()
        {
            string[] function = 
            {
                $"{_use32}",
                $"sub {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Right Parameter
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Left Parameter (Pushing Right Parameter Offset Stack Pointer)
                $"{Utilities.GetAbsoluteCallMnemonics(ManagedCalculator.Add, out _reverseWrapAddFunction)}",
                $"add {_esp}, {IntPtr.Size * 2}",
                $"add {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"ret"
            };

            _addFunction = new Function<ManagedCalculator.AddFunction>((long) CreateAsmFunction(function), ReloadedHooks.Instance);
        }

        private void CreateSubFunction()
        {
            string[] function =
            {
                $"{_use32}",
                $"sub {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Right Parameter
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Left Parameter (Pushing Right Parameter Offset Stack Pointer)
                $"{Utilities.GetAbsoluteCallMnemonics(ManagedCalculator.Subtract, out _reverseWrapSubFunction)}",
                $"add {_esp}, {IntPtr.Size * 2}",
                $"add {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"ret"
            };

            _subtractFunction = new Function<ManagedCalculator.SubtractFunction>((long)CreateAsmFunction(function), ReloadedHooks.Instance);
        }

        private void CreateMulFunction()
        {
            string[] function =
            {
                $"{_use32}",
                $"sub {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Right Parameter
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Left Parameter (Pushing Right Parameter Offset Stack Pointer)
                $"{Utilities.GetAbsoluteCallMnemonics(ManagedCalculator.Multiply, out _reverseWrapMulFunction)}",
                $"add {_esp}, {IntPtr.Size * 2}",
                $"add {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"ret"
            };

            _multiplyFunction = new Function<ManagedCalculator.MultiplyFunction>((long)CreateAsmFunction(function), ReloadedHooks.Instance);
        }

        private void CreateDivFunction()
        {
            string[] function =
            {
                $"{_use32}",
                $"sub {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Right Parameter
                $"push {_word} [{_esp} + {IntPtr.Size * 3}]", // Left Parameter (Pushing Right Parameter Offset Stack Pointer)
                $"{Utilities.GetAbsoluteCallMnemonics(ManagedCalculator.Divide, out _reverseWrapDivFunction)}",
                $"add {_esp}, {IntPtr.Size * 2}",
                $"add {_esp}, {IntPtr.Size}", // Preserve stack alignment for x64
                $"ret"
            };

            _divideFunction = new Function<ManagedCalculator.DivideFunction>((long)CreateAsmFunction(function), ReloadedHooks.Instance);
        }

        private IntPtr CreateAsmFunction(string[] function)
        {
            var result  = _assembler.Assemble(function);
            var address = _memory.Allocate(result.Length);
            _memory.WriteRaw(address, result);
            return address;
        }
    }
}
