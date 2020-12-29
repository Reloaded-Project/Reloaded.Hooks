using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Reloaded.Hooks.Definitions.Structs;

namespace Reloaded.Hooks.Definitions
{
    /// <summary>
    /// A user facing interface to obtain access to utility functions.
    /// </summary>
    public interface IReloadedHooksUtilities
    {
        /// <summary>
        /// Assembles an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        byte[] AssembleAbsoluteJump(IntPtr target, bool is64bit);

        /// <summary>
        /// Assembles a push + return combination to a given target address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        byte[] AssemblePushReturn(IntPtr target, bool is64bit);

        /// <summary>
        /// Assembles a relative (to EIP/RIP) jump by a user specified offset.
        /// </summary>
        /// <param name="relativeJumpOffset">Offset relative to EIP/RIP to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        byte[] AssembleRelativeJump(IntPtr relativeJumpOffset, bool is64bit);

        /// <summary>
        /// Assembles a relative (to EIP/RIP) jump by a user specified offset.
        /// </summary>
        /// <param name="currentAddress">Address of the current instruction.</param>
        /// <param name="targetAddress">The address to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        byte[] AssembleRelativeJump(IntPtr currentAddress, IntPtr targetAddress, bool is64bit);

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        string GetAbsoluteJumpMnemonics(IntPtr target, bool is64bit);

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute call to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        string GetAbsoluteCallMnemonics(IntPtr target, bool is64bit);

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a C# function address.
        /// </summary>
        /// <param name="function">The C# function to create a jump to.</param>
        /// <param name="reverseWrapper">
        ///     The native reverse wrapper used to call your function.
        ///     Please keep a reference to this class as long as you are using the generated code.
        ///     i.e. make it a class/struct member on heap.
        /// </param>
        string GetAbsoluteJumpMnemonics<TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate;

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute call to a C# function address.
        /// </summary>
        /// <param name="function">The C# function to create a jump to.</param>
        /// <param name="reverseWrapper">
        ///     The native reverse wrapper used to call your function.
        ///     Please keep a reference to this class as long as you are using the generated code.
        ///     i.e. make it a class/struct member on heap.
        /// </param>
        string GetAbsoluteCallMnemonics<TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate;

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        string GetPushReturnMnemonics(IntPtr target, bool is64bit);

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble a relative jump to the current instruction pointer.
        /// </summary>
        /// <param name="relativeJumpOffset">Offset relative to EIP/RIP to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        string GetRelativeJumpMnemonics(IntPtr relativeJumpOffset, bool is64bit);

        /// <summary>
        /// Appends an absolute jump to the supplied opcodes and assembles the result, returning a pointer.
        /// </summary>
        /// <param name="jumpTarget">The address to jump to.</param>
        /// <param name="opcodes">Bytes representing existing assembly instructions.</param>
        /// <param name="is64bit">True for x64 else x86</param>
        /// <param name="targetAddress">[Optional] Target address within of which the wrapper should be placed in <see cref="maxDisplacement"/> range.</param>
        /// <param name="maxDisplacement">Maximum distance from the <see cref="targetAddress"/></param>
        IntPtr InsertJump(byte[] opcodes, bool is64bit, long jumpTarget, long targetAddress = 0, long maxDisplacement = Int32.MaxValue);

        /// <summary>
        /// Retrieves the length of the hook for trampoline, mid-function hooks etc.
        /// </summary>
        /// <param name="hookAddress">The address that is to be hooked.</param>
        /// <param name="hookLength">The minimum length of the hook, the length of our assembled bytes for the hook.</param>
        /// <param name="is64Bit">True if 64bit, else false.</param>
        int GetHookLength(IntPtr hookAddress, int hookLength, bool is64Bit);

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate Type.
        /// </summary>
        /// <param name="delegateType">A Type extracted from a Delegate.</param>
        /// <returns>Number of parameters for the supplied delegate type.</returns>
        int GetNumberofParameters(Type delegateType);

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate Type.
        /// Ignores float and double parameters.
        /// </summary>
        /// <param name="delegateType">A Type extracted from a Delegate.</param>
        /// <returns>Number of parameters for the supplied delegate type, without floats.</returns>
        int GetNumberofParametersWithoutFloats(Type delegateType);

        /// <summary>
        /// Retrieves the number of parameters for a type that inherits from <see cref="IFuncPtr"/>.
        /// Otherwise defaults to checking by type, assuming the type is a <see cref="Delegate"/>
        /// </summary>
        /// <typeparam name="TFunction">Type that inherits from <see cref="IFuncPtr"/>.</typeparam>
        /// <param name="value">Any non-null value.</param>
        int GetNumberofParameters<TFunction>(TFunction value);

        /// <summary>
        /// Retrieves the number of parameters for a type that inherits from <see cref="IFuncPtr"/>.
        /// Otherwise defaults to checking by type, assuming the type is a <see cref="Delegate"/>
        /// Ignores float and double parameters.
        /// </summary>
        /// <typeparam name="TFunction">Type that inherits from <see cref="IFuncPtr"/>.</typeparam>
        /// <param name="value">Any non-null value.</param>
        int GetNumberofParametersWithoutFloats<TFunction>(TFunction value);

        /// <summary>
        /// A macro for "push eax\npush ecx\npush edx" that preserves all CDECL caller saved registers before
        /// a function call.
        /// </summary>
        string PushCdeclCallerSavedRegisters();

        /// <summary>
        /// A macro for "pop edx\npop ecx\npop eax" for safely restoring caller saved registers after a function call.
        /// </summary>
        string PopCdeclCallerSavedRegisters();

        /// <summary>
        /// Allocates a pointer to a given target address in unmanaged, non-reclaimable memory.
        /// </summary>
        /// <param name="target">The target address/value the pointer is pointing to</param>
        /// <returns>Address of the pointer.</returns>
        IntPtr WritePointer(IntPtr target);

        /*
         * -----------------
         * Private Functions
         * -----------------
         */

        /// <summary>
        /// Gets the minimum and maximum address possible with a relative jump.
        /// </summary>
        /// <param name="targetAddress">Address we are jumping from.</param>
        /// <param name="maxDisplacement">Maximum distance we can jump.</param>
        (long min, long max) GetRelativeJumpMinMax(long targetAddress, long maxDisplacement = Int32.MaxValue);
    }
}