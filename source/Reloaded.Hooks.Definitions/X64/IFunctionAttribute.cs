using System;
using System.Collections.Generic;
using System.Text;

namespace Reloaded.Hooks.Definitions.X64
{
    public interface IFunctionAttribute
    {
        /// <summary>
        /// Registers in left to right parameter order passed to the custom function.
        /// </summary>
        FunctionAttribute.Register[] SourceRegisters { get; }

        /// <summary>
        /// The register that the function returns its value in.
        /// This is typically rax.
        /// </summary>
        FunctionAttribute.Register ReturnRegister { get; }

        /// <summary>
        /// Defines whether the function to be called or hooked expects "Shadow Space".
        /// Shadow space allocates 32 bytes of memory onto the stack before calling the function, such that they
        /// may be used locally within the target function as storage.
        /// [Default] True: Microsoft X64 based calling conventions.
        /// False: SystemV-based calling conventions.
        /// </summary>
        bool ShadowSpace { get; }

        /// <summary>
        /// Specifies all the registers whose values are expected to be preserved by the function.
        /// </summary>
        FunctionAttribute.Register[] CalleeSavedRegisters { get; }
    }
}
