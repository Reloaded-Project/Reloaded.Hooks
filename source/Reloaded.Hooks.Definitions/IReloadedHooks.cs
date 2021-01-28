using System;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.X86;

namespace Reloaded.Hooks.Definitions
{
    /// <summary>
    /// An interface providing high level functionality for the Reloaded.Hooks library.
    /// </summary>
    public interface IReloadedHooks
    {
        /// <summary>
        /// Constructs an <see cref="IFunction{TFunction}"/>.
        /// Utility class which allows you to more easily hook or call a native function.
        /// </summary>
        /// <param name="address">The address of the function.</param>
        /// <typeparam name="TFunction">The delegate type of the function.</typeparam>
        /// <returns>The function.</returns>
        IFunction<TFunction> CreateFunction<TFunction>(long address);

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="function">The function to detour the original function to.</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>

        IHook<TFunction> CreateHook<TFunction>(TFunction function, long functionAddress, int minHookLength = -1);

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="targetAddress">The address to redirect the function to.</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Minimum hook length.</param>
        public unsafe IHook<TFunction> CreateHook<TFunction>(void* targetAddress, long functionAddress, int minHookLength = -1);

        /// <summary>
        /// Creates a hook detouring the provided function to a given function.
        /// Use only in .NET 5 and above with methods declared [UnmanagedCallersOnly].
        /// </summary>
        /// <param name="type">The type containing the method. Use "typeof()"</param>
        /// <param name="methodName">The name of the method. Use nameof()</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Minimum hook length.</param>
        public unsafe IHook<TFunction> CreateHook<TFunction>(Type type, string methodName, long functionAddress, int minHookLength);

        /// <summary>
        /// Creates a hook detouring the provided function to a given function.
        /// Use only in .NET 5 and above with methods declared [UnmanagedCallersOnly].
        /// </summary>
        /// <param name="type">The type containing the method. Use "typeof()"</param>
        /// <param name="methodName">The name of the method. Use nameof()</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        public unsafe IHook<TFunction> CreateHook<TFunction>(Type type, string methodName, long functionAddress);

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to create a wrapper for.</param>
        /// <param name="wrapperAddress">Address of the wrapper used to call the original function.</param>
        /// <returns>The function ready to be called.</returns>
        TFunction CreateWrapper<TFunction>(long functionAddress, out IntPtr wrapperAddress);

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling
        /// convention using the convention of <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to create a wrapper for.</param>
        /// <returns>Function pointer to the wrapper in memory you can call using <typeparamref name="TFunction"/>'s calling convention.</returns>
        IntPtr CreateWrapper<TFunction>(long functionAddress);

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling
        /// convention using the convention of <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">The address of the function.</param>
        /// <param name="fromConvention">Describes the calling convention of the function to wrap.</param>
        /// <returns>Function pointer to the wrapper in memory you can call using <typeparamref name="TFunction"/>'s calling convention.</returns>
        IntPtr CreateNativeWrapperX86<TFunction>(IntPtr functionAddress, IFunctionAttribute fromConvention);

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling
        /// convention using the convention of <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">The address of the function using <paramref name="fromConvention"/>.</param>
        /// <param name="fromConvention">The calling convention to convert to <paramref name="toConvention"/>. This is the convention of the function (<paramref name="functionAddress"/>) called.</param>
        /// <param name="toConvention">The target convention to which convert to <paramref name="fromConvention"/>. This is the convention of the function returned.</param>
        /// <returns>Address of the wrapper in memory you can call.</returns>
        IntPtr CreateNativeWrapperX86<TFunction>(IntPtr functionAddress, IFunctionAttribute fromConvention, IFunctionAttribute toConvention);

        /// <summary>
        /// Creates a wrapper function converting a call to a source calling convention to a given target calling convention.
        /// </summary>
        /// <param name="functionAddress">The address of the function using <paramref name="fromConvention"/>.</param>
        /// <param name="fromConvention">The calling convention to convert to <paramref name="toConvention"/>. This is the convention of the function (<paramref name="functionAddress"/>) called.</param>
        /// <param name="toConvention">The target convention to which convert to <paramref name="fromConvention"/>. This is the convention of the function returned.</param>
        /// <returns>Address of the wrapper in memory you can call.</returns>
        IntPtr CreateNativeWrapperX64<TFunction>(IntPtr functionAddress, Definitions.X64.IFunctionAttribute fromConvention, Definitions.X64.IFunctionAttribute toConvention);

        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it (if <typeparamref name="TFunction"/> is a delegate type).
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">The function to be called by the wrapper.</param>
        IReverseWrapper<TFunction> CreateReverseWrapper<TFunction>(TFunction function);

        /// <summary>
        /// Creates a wrapper function with a custom calling convention which calls the supplied function.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it (if <typeparamref name="TFunction"/> is a delegate type).
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">Pointer of native function to wrap.</param>
        IReverseWrapper<TFunction> CreateReverseWrapper<TFunction>(IntPtr function);

        /// <summary>
        /// Initiates a virtual function table from an object address in memory.
        /// An assumption is made that the virtual function table pointer is the first parameter.
        /// </summary>
        /// <param name="objectAddress">
        ///     The memory address at which the object is stored.
        ///     The function will assume that the first entry is a pointer to the virtual function
        ///     table, as standard with C++ code.
        /// </param>
        /// <param name="numberOfMethods">
        ///     The number of methods contained in the virtual function table.
        ///     For enumerables, you may obtain this value as such: Enum.GetNames(typeof(MyEnum)).Length; where
        ///     MyEnum is the name of your enumerable.
        /// </param>
        IVirtualFunctionTable VirtualFunctionTableFromObject(IntPtr objectAddress, int numberOfMethods);

        /// <summary>
        /// Initiates a virtual function table given the address of the first function in memory.
        /// </summary>
        /// <param name="tableAddress">
        ///     The memory address of the first entry (function pointer) of the virtual function table.
        /// </param>
        /// <param name="numberOfMethods">
        ///     The number of methods contained in the virtual function table.
        ///     For enumerables, you may obtain this value as such: Enum.GetNames(typeof(MyEnum)).Length; where
        ///     MyEnum is the name of your enumerable.
        /// </param>
        IVirtualFunctionTable VirtualFunctionTableFromAddress(IntPtr tableAddress, int numberOfMethods);

        /// <summary>
        /// Creates a pointer to a native function.
        /// </summary>
        IFunctionPtr<TDelegate> CreateFunctionPtr<TDelegate>(ulong functionPointer) where TDelegate : Delegate;

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">
        ///     The assembly code to execute, in FASM syntax.
        ///     (Should start with use32/use64)
        /// </param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        IAsmHook CreateAsmHook(string[] asmCode, long functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1);

        /// <summary>
        /// Creates a cheat engine style hook, replacing instruction(s) with a JMP to a user provided set of ASM instructions (and optionally the original ones).
        /// </summary>
        /// <param name="asmCode">The assembly code to execute, precompiled.</param>
        /// <param name="functionAddress">The address of the function or mid-function to hook.</param>
        /// <param name="behaviour">Defines what should be done with the original code that was replaced with the JMP instruction.</param>
        /// <param name="hookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        IAsmHook CreateAsmHook(byte[] asmCode, long functionAddress, AsmHookBehaviour behaviour = AsmHookBehaviour.ExecuteFirst, int hookLength = -1);

        /// <summary>
        /// Provides access to various useful utilities.
        /// </summary>
        IReloadedHooksUtilities Utilities { get; }
    }
}
