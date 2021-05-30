using System.Runtime.InteropServices;

namespace Reloaded.Hooks.Definitions.X86
{
    /// <summary>
    /// Overrides the expected calling convention of the native C# function called.
    /// This attribute is functionally equivalent to <see cref="UnmanagedFunctionPointerAttribute"/> used with delegates; but for Hooks.
    ///
    /// This function only affects the <see cref="IHook"/> and <see cref="IReverseWrapper"/> APIs.
    /// </summary>
    public class ManagedFunctionAttribute : FunctionAttribute
    {
        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace = 0) : base(sourceRegisters, returnRegister, stackCleanup, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace = 0) : base(sourceRegisters, returnRegister, stackCleanup, calleeSavedRegisters, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace = 0) : base(sourceRegister, returnRegister, stackCleanup, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace = 0) : base(sourceRegister, returnRegister, stackCleanup, calleeSavedRegisters, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(CallingConventions callingConvention) : base(callingConvention) { }
    }
}
