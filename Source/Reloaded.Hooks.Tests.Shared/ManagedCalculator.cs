using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Interop;
using CallingConventions = Reloaded.Hooks.Definitions.X86.CallingConventions;

namespace Reloaded.Hooks.Tests.Shared
{
    public class ManagedCalculator
    {
        /* Methods */
        public int Add(int a, int b) => a + b;
        public int Subtract(int a, int b) => a - b;
        public int Multiply(int a, int b) => a * b;
        public int Divide(int a, int b) => a / b;

        /// <summary> Performs A + B</summary>
        [Function(Definitions.X64.CallingConventions.Microsoft)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int AddFunction(int a, int b);

        /// <summary> Performs A - B. </summary>
        [Function(Definitions.X64.CallingConventions.Microsoft)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int SubtractFunction(int a, int b);

        /// <summary> Multiply A by B. </summary>
        [Function(Definitions.X64.CallingConventions.Microsoft)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int MultiplyFunction(int a, int b);

        /// <summary> Divide A by B. </summary>
        [Function(Definitions.X64.CallingConventions.Microsoft)]
        [Definitions.X86.Function(CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int DivideFunction(int a, int b);
    }
}
