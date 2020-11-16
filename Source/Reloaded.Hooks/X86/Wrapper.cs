using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.Internal;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;
using static Reloaded.Hooks.Definitions.X86.FunctionAttribute;

using Reloaded.Hooks.Definitions.Structs;

namespace Reloaded.Hooks.X86
{
    /// <summary>
    /// Allows for creating wrapper functions allow you to call functions with custom calling conventions using the calling convention of a given delegate.
    /// </summary>
    public static class Wrapper
    {
        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// using the calling convention of <see cref="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        public static TFunction Create<TFunction>(long functionAddress)
        {
            return Create<TFunction>(functionAddress, out var wrapperAddress);
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <see cref="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the original function is CDECL, the wrapper address equals the function address.
        /// </param>
        public static TFunction Create<TFunction>(long functionAddress, out IntPtr wrapperAddress)
        {
            return Marshal.GetDelegateForFunctionPointer<TFunction>(CreatePointer<TFunction>(functionAddress, out wrapperAddress));
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <see cref="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the original function is CDECL, the wrapper address equals the function address.
        /// </param>
        /// <returns>Address of the wrapper in native memory.</returns>
        public static IntPtr CreatePointer<TFunction>(long functionAddress, out IntPtr wrapperAddress)
        {
            var attribute = GetAttribute<TFunction>();
            wrapperAddress = (IntPtr)functionAddress;

            // Hot path: Don't create wrapper if both conventions are already compatible.
            var funcPtrAttribute = Misc.TryGetAttributeOrDefault<TFunction, UnmanagedFunctionPointerAttribute>();
            if (!attribute.IsEquivalent(funcPtrAttribute))
                wrapperAddress = Create<TFunction>((IntPtr)functionAddress, attribute, attribute.GetEquivalent(funcPtrAttribute));

            return wrapperAddress;
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <see cref="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to wrap.</param>
        /// <param name="wrapperAddress">Address of the wrapper used to call the original function.</param>
        public static TFunction CreateFunctionPointer<TFunction>(long functionAddress, out IntPtr wrapperAddress)
        {
            CreatePointer<TFunction>(functionAddress, out wrapperAddress);
            return Unsafe.As<IntPtr, TFunction>(ref wrapperAddress);
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <see cref="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">The address of the function using <see cref="fromConvention"/>.</param>
        /// <param name="fromConvention">The calling convention to convert to <see cref="toConvention"/>. This is the convention of the function (<see cref="functionAddress"/>) called.</param>
        /// <param name="toConvention">The target convention to which convert to <see cref="fromConvention"/>. This is the convention of the function returned.</param>
        /// <returns>Address of the wrapper in memory.</returns>
        public static IntPtr Create<TFunction>(IntPtr functionAddress, IFunctionAttribute fromConvention, IFunctionAttribute toConvention)
        {
            // toFunction (target) is CDECL
            int numberOfParameters    = Utilities.GetNumberofParameters(typeof(TFunction));
            List<string> assemblyCode = new List<string> {"use32"};

            // Calculate some stack stuff.
            int fromStackParamBytesTotal = (fromConvention.Cleanup == StackCleanup.Caller) ? (numberOfParameters - fromConvention.SourceRegisters.Length) * 4 : 0;
            int toStackParamBytesTotal   = (toConvention.Cleanup == StackCleanup.Callee)   ? (numberOfParameters - toConvention.SourceRegisters.Length) * 4 : 0;
            int stackCleanupBytesTotal   = fromStackParamBytesTotal + fromConvention.ReservedStackSpace;

            // Callee Saved Registers
            assemblyCode.Add("push ebp");       // Backup old call frame
            assemblyCode.Add("mov ebp, esp");   // Setup new call frame
            foreach (var register in toConvention.CalleeSavedRegisters)
                assemblyCode.Add($"push {register}");

            // Reserve Extra Stack Space
            if (fromConvention.ReservedStackSpace > 0)
                assemblyCode.Add($"sub esp, {fromConvention.ReservedStackSpace}");

            // Setup Function Parameters
            if (numberOfParameters > 0)
                assemblyCode.AddRange(AssembleFunctionParameters(numberOfParameters, fromConvention.SourceRegisters, toConvention.SourceRegisters));

            // Call target function
            var pointerBuffer = Utilities.FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr targetFunctionPtr = pointerBuffer.Add(ref functionAddress);
            assemblyCode.Add("call dword [0x" + targetFunctionPtr.ToString("X") + "]"); 

            // Stack cleanup if necessary 
            if (stackCleanupBytesTotal > 0)
                assemblyCode.Add($"add esp, {stackCleanupBytesTotal}");

            // Setup return register
            if (fromConvention.ReturnRegister != toConvention.ReturnRegister)
                assemblyCode.Add($"mov {toConvention.ReturnRegister}, {fromConvention.ReturnRegister}");

            // Callee Restore Registers
            foreach (var register in toConvention.CalleeSavedRegisters.Reverse())
                assemblyCode.Add($"pop {register}");

            assemblyCode.Add("pop ebp");
            assemblyCode.Add($"ret {toStackParamBytesTotal}"); // FASM optimizes `ret 0` as `ret`
            
            // Write function to buffer and return pointer.
            byte[] assembledMnemonics = Utilities.Assembler.Assemble(assemblyCode.ToArray());
            var wrapperBuffer = Utilities.FindOrCreateBufferInRange(assembledMnemonics.Length);
            return wrapperBuffer.Add(assembledMnemonics);
        }

        private static string[] AssembleFunctionParameters(int parameterCount, Register[] fromRegisters, Register[] toRegisters)
        {
            List<string> assemblyCode = new List<string>();

            /*
               At the current moment in time,
               The base address of old call stack (EBP) is at [ebp + 0]
               The return address of the calling function is at [ebp + 4]
               Last parameter is therefore at [ebp + 8].

               Note: Reason return address is not at [ebp + 0] is because we pushed ebp and mov'd esp to it.
               Reminder: The stack grows by DECREMENTING THE STACK POINTER.
            
               Note 2: Don't need to account for reserved stack space in toConvention because we reserve it before pushing the parameters.
             */

            // The initial offset from EBP (Stack Base Pointer) for the rightmost parameter (right to left passing):
            int toStackParams   = parameterCount - toRegisters.Length;          // Stack parameter count in toConvention
            int baseStackOffset = ((toStackParams) * 4) + 4;                    // + 4 because base address of old call stack is currently at ebp + 0

            // Re-push all toConvention stack params, then register parameters. (Right to Left)
            for (int x = 0; x < toStackParams; x++)
            {
                assemblyCode.Add($"push dword [ebp + {baseStackOffset}]");
                baseStackOffset -= 4;
            }

            for (int x = Math.Min(toRegisters.Length, parameterCount) - 1; x >= 0; x--)
                assemblyCode.Add($"push {toRegisters[x]}");

            // Now pop all necessary registers to target. (Left to Right)
            for (int x = 0; x < fromRegisters.Length && x < parameterCount; x++)
                assemblyCode.Add($"pop {fromRegisters[x]}");

            return assemblyCode.ToArray();
        }
    }
}
