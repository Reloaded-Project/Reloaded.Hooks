/*
    [Reloaded] Mod Loader Common Library (libReloaded)
    The main library acting as common, shared code between the Reloaded Mod 
    Loader Launcher, Mods as well as plugins.
    Copyright (C) 2018  Sewer. Sz (Sewer56)

    [Reloaded] is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    [Reloaded] is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>
*/

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reloaded.Hooks.X86
{
    /// <summary>
    /// Stores function information for custom functions.
    /// See <see cref="CallingConventions" /> for information common calling convention settings.
    /// </summary>
    public class FunctionAttribute : Attribute
    {
        /// <summary>
        /// Registers in left to right parameter order passed to the custom function.
        /// </summary>
        public Register[] SourceRegisters { get; }

        /// <summary>
        /// The register that the function returns its value in.
        /// This is typically eax.
        /// </summary>
        public Register ReturnRegister { get; }

        /// <summary>
        /// Defines the stack cleanup rule for the function.
        /// Callee: Stack pointer restored inside the function we are executing.
        /// Caller: Stack pointer restored in our own wrapper function.
        /// </summary>
        public StackCleanup Cleanup { get; }

        /// <summary>
        /// Used for allocating an extra amount of uninitialized (not zero-written) stack space 
        /// before calling the function. This is required by some compiler optimized functions.
        /// </summary>
        public int ReservedStackSpace { get; }

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Register
        {
            eax,
            ebx,
            ecx,
            edx,
            esi,
            edi,
            ebp,
            esp
        }
        
        public enum StackCleanup
        {
            None,
            Caller,
            Callee
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegisters">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="stackCleanup">Defines the stack cleanup rule for the function. See <see cref="StackCleanup"/> for more details.</param>
        /// <param name="reservedStackSpace">Allocates an extra amount of uninitialized (not zero-written) stack space for the function to use when calling. Required by some compiler optimized functions.</param>
        public FunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace = 0)
        {
            SourceRegisters = sourceRegisters;
            ReturnRegister = returnRegister;
            Cleanup = stackCleanup;
            ReservedStackSpace = reservedStackSpace;
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegister">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="stackCleanup">Defines the stack cleanup rule for the function. See <see cref="StackCleanup"/> for more details.</param>
        /// <param name="reservedStackSpace">Allocates an extra amount of uninitialized (not zero-written) stack space for the function to use when calling. Required by some compiler optimized functions.</param>
        public FunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace = 0)
        {
            SourceRegisters = new[] { sourceRegister };
            ReturnRegister = returnRegister;
            Cleanup = stackCleanup;
            ReservedStackSpace = reservedStackSpace;
        }

        /// <summary>
        /// Initializes the ReloadedFunction using a preset calling convention.
        /// </summary>
        /// <param name="callingConvention">
        ///     The calling convention preset to use for instantiating the ReloadedFunction.
        ///     Please remember to mark your function delegate as [UnmanagedFunctionPointer(CallingConvention.Cdecl)],
        ///     mark only the ReloadedFunction Attribute with the true calling convention.
        /// </param>
        public FunctionAttribute(CallingConventions callingConvention)
        {
            switch (callingConvention)
            {
                case CallingConventions.Cdecl:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    break;

                case CallingConventions.Stdcall:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Callee;
                    break;

                case CallingConventions.Fastcall:
                    SourceRegisters = new []{ Register.ecx, Register.edx };
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    break;

                case CallingConventions.MicrosoftThiscall:
                    SourceRegisters = new[] { Register.ecx };
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Callee;
                    break;

                case CallingConventions.GCCThiscall:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    break;

                default:
                    throw new ArgumentException($"There is no preset for the specified calling convention {callingConvention.GetType().Name}");
            }
        }

        /// <summary>
        /// Retrieves a ReloadedFunction attribute from a supplied delegate type.
        /// </summary>
        public static FunctionAttribute GetAttribute<TFunction>()
        {
            foreach (Attribute attribute in typeof(TFunction).GetCustomAttributes(false))
            {
                if (attribute is FunctionAttribute reloadedFunction)
                    return reloadedFunction;
            }

            throw new HookException($"ReloadedFunctionAttribute is missing in the {typeof(TFunction).Name} delegate declaration." +
                                    $"Please mark the {typeof(TFunction).Name} with an appropriate ReloadedFunctionAttribute");
        }

        /* Override Equals & GetHashCode */

        public override bool Equals(Object obj)
        {
            FunctionAttribute functionAttribute = obj as FunctionAttribute;

            if (functionAttribute == null)
                return false;
            
            return functionAttribute.Cleanup == Cleanup &&
                   functionAttribute.ReturnRegister == ReturnRegister &&
                   functionAttribute.SourceRegisters.SequenceEqual(SourceRegisters);
        }

        public override int GetHashCode()
        {
            int initialHash = 13;

            foreach (Register register in SourceRegisters)
            { initialHash = (initialHash * 7) + (int)register; }
            
            initialHash = (initialHash * 7) + (int)ReturnRegister;
            initialHash = (initialHash * 7) + (int)Cleanup;

            return initialHash;
        }
    }
}
