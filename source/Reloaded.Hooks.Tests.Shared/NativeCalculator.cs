﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Tools;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;
using FunctionAttribute = Reloaded.Hooks.Definitions.X64.FunctionAttribute;

namespace Reloaded.Hooks.Tests.Shared
{
    /// <summary>
    /// Collection of simple plain X86/X64 ASM calculator functions.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public class NativeCalculator : IDisposable
    {
        /// <summary> Performs A + B</summary>
        [Function(new FunctionAttribute.Register[0] { }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        public delegate int AddFunction(int a, int b);

        /// <summary> Performs A - B. </summary>
        [Function(new FunctionAttribute.Register[0] { }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        public delegate int SubtractFunction(int a, int b);

        /// <summary> Multiply A by B. </summary>
        [Function(new FunctionAttribute.Register[0] { }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        public delegate int MultiplyFunction(int a, int b);

        /// <summary> Divide A by B. </summary>
        [Function(new FunctionAttribute.Register[0] { }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        public delegate int DivideFunction(int a, int b);

        [Function(new FunctionAttribute.Register[0] { }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        public struct CalculatorFunction { public FuncPtr<int, int, int> Value; }

        public IntPtr Divide   { get; private set; }
        public IntPtr Multiply { get; private set; }
        public IntPtr Subtract { get; private set; }
        public IntPtr Add      { get; private set; }
        public IntPtr AddWithBranch { get; private set; }

        public IntPtr VTable   { get; private set; }

        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        private Memory.Sources.Memory _memory  = new Memory.Sources.Memory();

        /* For testing VTable */
        public enum VTableFunctions
        {
            Add,
            Subtract,
            Multiply,
            Divide
        }

        /* Constructor and Destructor */

        public NativeCalculator()
        {
            BuildAdd();
            BuildSubtract();
            BuildDivide();
            BuildMultiply();
            BuildVTable();

            BuildAddWithBranch();
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            _memory.Free(Add);
            _memory.Free(Subtract);
            _memory.Free(Multiply);
            _memory.Free(Divide);
            _assembler.Dispose();
        }

        /* Building functions to hook. */

        private void BuildAdd()
        {
            int wordSize = IntPtr.Size;
            string[] addFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
                $"add {_eax}, {_ecx}",

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(addFunction);
            Add = _memory.Allocate(result.Length);
            _memory.WriteRaw(Add, result);
        }

        private void BuildMultiply()
        {
            int wordSize = IntPtr.Size;
            string[] multiplyFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
                $"imul {_eax}, {_ecx}",

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(multiplyFunction);
            Multiply = _memory.Allocate(result.Length);
            _memory.WriteRaw(Multiply, result);
        }

        private void BuildDivide()
        {
            int wordSize = IntPtr.Size;
            string[] divideFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_edx}, 0",                         // Ignore the upper bits for testing.
                $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
                $"idiv {_ecx}",

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(divideFunction);
            Divide = _memory.Allocate(result.Length);
            _memory.WriteRaw(Divide, result);
        }

        private void BuildSubtract()
        {
            int wordSize = IntPtr.Size;
            string[] subtractFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
                $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
                $"sub {_eax}, {_ecx}",

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(subtractFunction);
            Subtract = _memory.Allocate(result.Length);
            _memory.WriteRaw(Subtract, result);
        }

        private void BuildVTable()
        {
            int size = Enum.GetNames(typeof(VTableFunctions)).Length * IntPtr.Size;
            var buffer = Utilities.FindOrCreateBufferInRange(size);

            IntPtr add = Add;
            IntPtr subtract = Subtract;
            IntPtr multiply = Multiply;
            IntPtr divide = Divide;

            VTable = buffer.Add(ref add, false, 1);
            buffer.Add(ref subtract, false, 1);
            buffer.Add(ref multiply, false, 1);
            buffer.Add(ref divide, false, 1);
        }

        /* Extra tests for Iced branch patching. */
        private void BuildAddWithBranch()
        {
            int wordSize = IntPtr.Size;
            string[] addFunction = new string[]
            {
                $"{_use32}",
                "jmp actualfunction",

                // Section of NOPs, iced must successfully manage to patch the jump over these.
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"nop",
                $"actualfunction:",
                $"mov {_eax}, [{_esp} + {wordSize * 1}]", // Left Parameter
                $"mov {_ecx}, [{_esp} + {wordSize * 2}]", // Right Parameter
                $"add {_eax}, {_ecx}",

                $"ret"
            };

            var result = _assembler.Assemble(addFunction);
            AddWithBranch = _memory.Allocate(result.Length);
            _memory.WriteRaw(AddWithBranch, result);
        }
    }
}
