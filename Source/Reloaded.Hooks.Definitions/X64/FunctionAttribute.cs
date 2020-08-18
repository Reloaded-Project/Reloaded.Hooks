using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Reloaded.Hooks.Definitions.X64
{
    /// <summary>
    /// Stores function information for custom functions.
    /// See <see cref="CallingConventions" /> for information common calling convention settings.
    /// </summary>
    public class FunctionAttribute : Attribute, IFunctionAttribute
    {
        /// <summary>
        /// Registers in left to right parameter order passed to the custom function.
        /// </summary>
        public Register[] SourceRegisters { get; }

        /// <summary>
        /// The register that the function returns its value in.
        /// This is typically rax.
        /// </summary>
        public Register ReturnRegister { get; }

        /// <summary>
        /// Defines whether the function to be called or hooked expects "Shadow Space".
        /// Shadow space allocates 32 bytes of memory onto the stack before calling the function, such that they
        /// may be used locally within the target function as storage.
        /// [Default] True: Microsoft X64 based calling conventions.
        /// False: SystemV-based calling conventions.
        /// </summary>
        public bool ShadowSpace { get; } = true;
        
        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Register
        {
            rax,
            rbx,
            rcx,
            rdx,
            rsi,
            rdi,
            rbp,
            rsp,
            r8,
            r9,
            r10,
            r11,
            r12,
            r13,
            r14,
            r15
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegisters">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="shadowSpace">
        ///     [Default = true] Defines whether the function to be called or hooked expects "Shadow Space".
        ///     Shadow space allocates 32 bytes of memory onto the stack before calling the function. See class definition for more details.
        /// </param>
        public FunctionAttribute(Register[] sourceRegisters, Register returnRegister, bool shadowSpace)
        {
            SourceRegisters = sourceRegisters;
            ReturnRegister = returnRegister;
            ShadowSpace = shadowSpace;
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegister">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="shadowSpace">
        ///     [Default = true] Defines whether the function to be called or hooked expects "Shadow Space".
        ///     Shadow space allocates 32 bytes of memory onto the stack before calling the function. See class definition for more details.
        /// </param>
        public FunctionAttribute(Register sourceRegister, Register returnRegister, bool shadowSpace)
        {
            SourceRegisters = new[] { sourceRegister };
            ReturnRegister = returnRegister;
            ShadowSpace = shadowSpace;
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
                case CallingConventions.Microsoft:
                    SourceRegisters = new [] { Register.rcx, Register.rdx, Register.r8, Register.r9 };
                    ReturnRegister = Register.rax;
                    ShadowSpace = true;
                    break;

                case CallingConventions.SystemV:
                    SourceRegisters = new [] { Register.rdi, Register.rsi, Register.rdx, Register.rcx, Register.r8, Register.r9 };
                    ReturnRegister = Register.rax;
                    ShadowSpace = false;
                    break;

                default:
                    throw new ArgumentException($"There is no preset for the specified calling convention {callingConvention.GetType().Name}");
            }
        }


        /// <summary>
        /// Retrieves a ReloadedFunction from a supplied delegate type.
        /// </summary>
        public static IFunctionAttribute GetAttribute<TFunction>()
        {
            foreach (Attribute attribute in typeof(TFunction).GetCustomAttributes(false))
            {
                if (attribute is IFunctionAttribute reloadedFunction)
                    return reloadedFunction;
            }
            
            return new FunctionAttribute(CallingConventions.Microsoft);
        }

        /* Override Equals & GetHashCode: ReSharper Generated */

        [ExcludeFromCodeCoverage]
        public override bool Equals(Object obj)
        {
            FunctionAttribute functionAttribute = obj as FunctionAttribute;

            if (functionAttribute == null) return false;
            
            return functionAttribute.ShadowSpace == ShadowSpace &&
                   functionAttribute.ReturnRegister == ReturnRegister &&
                   functionAttribute.SourceRegisters.SequenceEqual(SourceRegisters);
        }
        
        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            int initialHash = 13;

            foreach (Register register in SourceRegisters)
                initialHash = (initialHash * 7) + (int)register;
            
            initialHash = (initialHash * 7) + (int)ReturnRegister;
            initialHash = (initialHash * 7) + ShadowSpace.GetHashCode();

            return initialHash;
        }
    }
}
