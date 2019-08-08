using System;
using System.Collections.Generic;
using System.Text;

namespace Reloaded.Hooks.Definitions.X86
{
    public interface IFunctionAttribute
    {
        /// <summary>
        /// Registers in left to right parameter order passed to the custom function.
        /// </summary>
        FunctionAttribute.Register[] SourceRegisters { get; }

        /// <summary>
        /// The register that the function returns its value in.
        /// This is typically eax.
        /// </summary>
        FunctionAttribute.Register ReturnRegister { get; }

        /// <summary>
        /// Defines the stack cleanup rule for the function.
        /// Callee: Stack pointer restored inside the function we are executing.
        /// Caller: Stack pointer restored in our own wrapper function.
        /// </summary>
        FunctionAttribute.StackCleanup Cleanup { get; }

        /// <summary>
        /// Used for allocating an extra amount of uninitialized (not zero-written) stack space 
        /// before calling the function. This is required by some compiler optimized functions.
        /// </summary>
        int ReservedStackSpace { get; }
    }

}
