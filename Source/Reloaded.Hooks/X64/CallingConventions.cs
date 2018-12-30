namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// This class provides information on various commonly seen calling conventions.
    /// </summary>
    public enum CallingConventions
    {
        /// <summary>
        /// Parameters are passed in the order of RCX, RDX, R8, R9 registers, left to right.
        /// Remaining parameters are passed right to left onto the function pushing onto the stack.
        ///
        /// Caller's responsibility to create allocate 32 bytes of "shadow space" on the stack before calling function.
        /// 
        /// Calling function pops its own arguments from the stack if necessary and uses the "shadow space"
        /// as storage for the individual parameters to free registers if necessary.
        ///
        /// The calling function must manually restore the stack to previous state
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    RCX, RDX, R8, R9
        ///     ReturnRegister:     RAX    
        ///     Cleanup:            Caller
        /// </summary>
        Microsoft,

        /// <summary>
        /// Parameters are passed in the order of RDI, RSI, RDX, RCX, R8, R9 registers, left to right.
        /// Remaining parameters are passed right to left onto the function pushing onto the stack.
        /// 
        /// No necessity of "shadow space" is provided, though Reloaded will provide it anyway for
        /// compatibility with custom conventions.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    RDI, RSI, RDX, RCX, R8, R9 
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Callee
        /// </summary>
        SystemV,

        /// <summary>
        /// Placeholder for custom, compiler optimized calling conventions which don't 
        /// follow any particular standard.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    Depends on Function
        ///     ReturnRegister:     Depends on Function
        ///     Cleanup:            Depends on Function
        /// </summary>
        Custom,
    }
}
