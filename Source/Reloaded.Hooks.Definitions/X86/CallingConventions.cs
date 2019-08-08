namespace Reloaded.Hooks.Definitions.X86
{
    /// <summary>
    /// This class provides information on various commonly seen calling conventions and how
    /// to call functions utilising them.
    /// </summary>
    public enum CallingConventions
    {
        /// <summary>
        /// Parameters are passed right to left onto the function pushing onto the stack.
        /// Calling function pops its own arguments from the stack. 
        /// (The calling function must manually restore the stack to previous state)
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    N/A
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Caller
        /// </summary>
        Cdecl,

        /// <summary>
        /// Parameters are passed right to left onto the function pushing onto the stack.
        /// Called function pops its own arguments from the stack.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    N/A
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Callee
        /// </summary>
        Stdcall,

        /// <summary>
        /// The first two arguments are passed in from left to right into ECX and EDX.
        /// The others are passed in right to left onto stack.
        ///
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    ECX, EDX
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Caller
        /// 
        /// Caller cleanup: If necessary, the stack is cleaned up by the caller.
        /// </summary>
        Fastcall,

        /// <summary>
        /// Variant of Stdcall where the pointer of the `this` object is passed into ECX and
        /// rest of the parameters passed as usual. The Callee cleans the stack.
        ///
        /// You should define your delegates with the (this) object pointer (IntPtr) as first parameter from the left.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    ECX
        ///     ReturnRegister:     EAX
        ///     Cleanup:            Callee
        /// 
        /// For GCC variant of Thiscall, use Cdecl.
        /// </summary>
        MicrosoftThiscall,

        /// <summary>
        /// A variant of CDECL whereby the first parameter is the pointer to the `this` object.
        /// Everything is otherwise the same.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    N/A
        ///     ReturnRegister:     EAX    
        ///     Cleanup:            Caller
        /// </summary>
        // ReSharper disable once InconsistentNaming
        GCCThiscall,

        /// <summary>
        /// A name given to custom calling conventions by Hex-Rays (IDA) that are cleaned up by the caller.
        /// You should declare the <see cref="FunctionAttribute"/> manually yourself.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    Depends on Function
        ///     ReturnRegister:     Depends on Function
        ///     Cleanup:            Caller
        /// </summary>
        Usercall,

        /// <summary>
        /// A name given to custom calling conventions by Hex-Rays (IDA) that are cleaned up by the callee.
        /// You should declare the <see cref="FunctionAttribute"/> manually yourself.
        /// 
        /// ReloadedFunction Attribute:
        ///     TargetRegisters:    Depends on Function
        ///     ReturnRegister:     Depends on Function
        ///     Cleanup:            Callee
        /// </summary>
        Userpurge
    }
}
