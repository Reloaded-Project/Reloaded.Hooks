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
        // CAREFUL EDITING THIS FILE, FOLLOW THE RULES AT https://github.com/dotnet/roslyn/blob/main/docs/Adding%20Optional%20Parameters%20in%20Public%20API.md

        // BACKCOMPAT OVERLOAD -- DO NOT TOUCH
        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup) : base(sourceRegisters, returnRegister, stackCleanup, 0) { }

        // BACKCOMPAT OVERLOAD -- DO NOT TOUCH
        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters) : base(sourceRegisters, returnRegister, stackCleanup, calleeSavedRegisters, 0) { }

        // BACKCOMPAT OVERLOAD -- DO NOT TOUCH
        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup) : base(sourceRegister, returnRegister, stackCleanup, 0) { }

        // BACKCOMPAT OVERLOAD -- DO NOT TOUCH
        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters) : base(sourceRegister, returnRegister, stackCleanup, calleeSavedRegisters, 0) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace) : base(sourceRegisters, returnRegister, stackCleanup, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace) : base(sourceRegisters, returnRegister, stackCleanup, calleeSavedRegisters, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace) : base(sourceRegister, returnRegister, stackCleanup, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace) : base(sourceRegister, returnRegister, stackCleanup, calleeSavedRegisters, reservedStackSpace) { }

        /// <inheritdoc />
        public ManagedFunctionAttribute(CallingConventions callingConvention) : base(callingConvention) { }
    }
}
