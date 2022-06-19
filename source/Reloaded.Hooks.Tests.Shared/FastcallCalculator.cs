using System;
using System.Diagnostics.CodeAnalysis;
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
    /// Collection of simple plain X86/X64 ASM calculator functions written
    /// in X86 Fastcall.
    /// </summary>
    [SuppressUnmanagedCodeSecurity]
    public class FastcallCalculator : IDisposable
    {
        /// <summary> Performs A + B</summary>
        [Function(new[] { FunctionAttribute.Register.rcx, FunctionAttribute.Register.rdx }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Fastcall)]
        public delegate int AddFunction(int a, int b);

        /// <summary> Performs A - B. </summary>
        [Function(new[] { FunctionAttribute.Register.rcx, FunctionAttribute.Register.rdx }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Fastcall)]
        public delegate int SubtractFunction(int a, int b);

        /// <summary> Multiply A by B. </summary>
        [Function(new[] { FunctionAttribute.Register.rcx, FunctionAttribute.Register.rdx }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Fastcall)]
        public delegate int MultiplyFunction(int a, int b);

        /// <summary> Divide A by B. </summary>
        [Function(new[] { FunctionAttribute.Register.rcx, FunctionAttribute.Register.rdx }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Fastcall)]
        public delegate int DivideFunction(int a, int b);

        [Function(new[] { FunctionAttribute.Register.rcx, FunctionAttribute.Register.rdx }, FunctionAttribute.Register.rax, false)]
        [Definitions.X86.Function(CallingConventions.Fastcall)]
        public struct CalculatorFunction { public FuncPtr<int, int, int> Value; }

        public nuint Divide   { get; private set; }
        public nuint Multiply { get; private set; }
        public nuint Subtract { get; private set; }
        public nuint Add      { get; private set; }

        public nuint VTable   { get; private set; }

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

        public FastcallCalculator()
        {
            BuildAdd();
            BuildSubtract();
            BuildDivide();
            BuildMultiply();
            BuildVTable();
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
            string[] addFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, {_edx}", // Right Parameter
                $"add {_eax}, {_ecx}", // Left Parameter

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(addFunction);
            Add = _memory.Allocate(result.Length);
            _memory.WriteRaw(Add, result);
        }

        private void BuildMultiply()
        {
            string[] multiplyFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, {_edx}",  // Right Parameter
                $"imul {_eax}, {_ecx}", // Left Parameter

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(multiplyFunction);
            Multiply = _memory.Allocate(result.Length);
            _memory.WriteRaw(Multiply, result);
        }

        private void BuildDivide()
        {
            string[] divideFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, {_ecx}",                    // Left Parameter
                $"mov {_ecx}, {_edx}",                    // Right Parameter
                $"xor {_edx}, {_edx}",
                $"idiv {_ecx}",                           // Right Parameter.

                $"pop {_ebp}",
                $"ret"
            };

            var result = _assembler.Assemble(divideFunction);
            Divide = _memory.Allocate(result.Length);
            _memory.WriteRaw(Divide, result);
        }

        private void BuildSubtract()
        {
            string[] subtractFunction = new string[]
            {
                $"{_use32}",
                $"push {_ebp}",
                $"mov {_ebp}, {_esp}",

                $"mov {_eax}, {_ecx}", // Left Parameter
                $"sub {_eax}, {_edx}", // Right Parameter

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

            nuint add = Add;
            nuint subtract = Subtract;
            nuint multiply = Multiply;
            nuint divide = Divide;

            VTable = buffer.Add(ref add, false, 1);
            buffer.Add(ref subtract, false, 1);
            buffer.Add(ref multiply, false, 1);
            buffer.Add(ref divide, false, 1);
        }
    }
}
