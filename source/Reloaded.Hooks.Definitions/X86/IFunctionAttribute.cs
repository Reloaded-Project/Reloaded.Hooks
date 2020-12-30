using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
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

        /// <summary>
        /// Specifies all the registers whose values are expected to be preserved by the function.
        /// </summary>
        FunctionAttribute.Register[] CalleeSavedRegisters { get; }

        /// <summary>
        /// Checks if the given attribute matches an already built-in convention.
        /// </summary>
        /// <param name="attribute">Existing supported attribute.</param>
        bool IsEquivalent(UnmanagedFunctionPointerAttribute attribute);

        /// <summary>
        /// If possible, gets an equivalent supported function attribute.
        /// </summary>
        /// <param name="attribute">Existing supported attribute.</param>
        IFunctionAttribute GetEquivalent(UnmanagedFunctionPointerAttribute attribute);
    }

}
