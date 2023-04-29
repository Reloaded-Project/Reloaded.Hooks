using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Internal;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;
using static Reloaded.Hooks.Definitions.X86.FunctionAttribute;

using Reloaded.Hooks.Definitions.Structs;
using System.Diagnostics.CodeAnalysis;
using Reloaded.Hooks.Definitions;

namespace Reloaded.Hooks.X86
{
    /// <summary>
    /// Allows for creating wrapper functions allow you to call functions with custom calling conventions using the calling convention of a given delegate.
    /// </summary>
    public static class Wrapper
    {
        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// using the calling convention of <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        public static TFunction Create<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(nuint functionAddress)
        {
            return Create<TFunction>(functionAddress, out var wrapperAddress);
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the source and target calling conventions match, this is the same as <paramref name="functionAddress"/>
        /// </param>
        public static TFunction Create<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(nuint functionAddress, out nuint wrapperAddress)
        {
            return Create<TFunction>(functionAddress, new WrapperOptions(), out wrapperAddress);
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="options">Options that influence wrapper generation.</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the source and target calling conventions match, this is the same as <paramref name="functionAddress"/>
        /// </param>
        public static TFunction Create<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
            TFunction>(nuint functionAddress, WrapperOptions options, out nuint wrapperAddress)
        {
            CreatePointer<TFunction>(functionAddress, options, out wrapperAddress);
            if (typeof(TFunction).IsValueType && !typeof(TFunction).IsPrimitive)
                return Unsafe.As<nuint, TFunction>(ref wrapperAddress);

            return Marshal.GetDelegateForFunctionPointer<TFunction>(wrapperAddress.ToSigned());
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="options">Influences wrapper generation.</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the original function is CDECL, the wrapper address equals the function address.
        /// </param>
        /// <returns>Address of the wrapper in native memory.</returns>
        public static nuint CreatePointer<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
            TFunction>(nuint functionAddress, WrapperOptions options, out nuint wrapperAddress)
        {
            var attribute = GetAttribute<TFunction>();
            wrapperAddress = functionAddress;

            // Hot path: Don't create wrapper if both conventions are already compatible.
            var funcPtrAttribute = Misc.TryGetAttributeOrDefault<TFunction, UnmanagedFunctionPointerAttribute>();
            if (!attribute.IsEquivalent(funcPtrAttribute))
                wrapperAddress = Create<TFunction>(functionAddress, attribute, attribute.GetEquivalent(funcPtrAttribute), options);

            return wrapperAddress;
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the original function is CDECL, the wrapper address equals the function address.
        /// </param>
        /// <returns>Address of the wrapper in native memory.</returns>
        public static nuint CreatePointer<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(nuint functionAddress, out nuint wrapperAddress)
        {
            return CreatePointer<TFunction>(functionAddress, new WrapperOptions(), out wrapperAddress);
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">The address of the function using <paramref name="fromConvention"/>.</param>
        /// <param name="fromConvention">The calling convention to convert to <paramref name="toConvention"/>. This is the convention of the function (<paramref name="functionAddress"/>) called.</param>
        /// <param name="toConvention">The target convention to which convert to <paramref name="fromConvention"/>. This is the convention of the function returned.</param>
        /// <returns>Address of the wrapper in memory.</returns>
        public static nuint Create<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(nuint functionAddress, IFunctionAttribute fromConvention, IFunctionAttribute toConvention)
        {
            return Create<TFunction>(functionAddress, fromConvention, toConvention, new WrapperOptions());
        }

        /// <summary>
        /// Creates a wrapper function which allows you to call a function with a custom calling convention using the calling convention of
        /// <typeparamref name="TFunction"/>.
        /// </summary>
        /// <param name="functionAddress">The address of the function using <paramref name="fromConvention"/>.</param>
        /// <param name="fromConvention">The calling convention to convert to <paramref name="toConvention"/>. This is the convention of the function (<paramref name="functionAddress"/>) called.</param>
        /// <param name="toConvention">The target convention to which convert to <paramref name="fromConvention"/>. This is the convention of the function returned.</param>
        /// <param name="options">Options that influence wrapper generation.</param>
        /// <returns>Address of the wrapper in memory.</returns>
        public static nuint Create<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(nuint functionAddress, IFunctionAttribute fromConvention, IFunctionAttribute toConvention, WrapperOptions options)
        {
            // 256 Bytes should allow for around 60-70 parameters in worst case scenario.
            // If you need more than that, then... I don't know what you're doing with your life.
            // Please do a pull request though and we can stick some code to predict the size.
            const int MaxFunctionSize = 256;
            var target = options.ProximityTarget;
            
            var usesCustomTarget = target != null;
            var targetAddress = usesCustomTarget ? target.TargetAddress : functionAddress;
            var functionSize = usesCustomTarget ? target.ItemSize : MaxFunctionSize;
            var targetProximity = usesCustomTarget ? target.RequestedProximity : Int32.MaxValue;
            
            var minMax = Utilities.GetRelativeJumpMinMax(targetAddress, targetProximity - functionSize);
            var buffer = Utilities.FindOrCreateBufferInRange(functionSize, minMax.min, minMax.max);
            var numberOfParameters = Utilities.GetNumberofParameters<TFunction>();

            return buffer.ExecuteWithLock(() =>
            {
                // Align the code.
                buffer.SetAlignment(16);
                var codeAddress = buffer.Properties.WritePointer;

                // Write pointer.
                // toFunction (target) is CDECL
                List<string> assemblyCode = new List<string>
                {
                    "use32",
                    $"org {codeAddress}" // Tells FASM where code should be located.
                };

                // Calculate some stack stuff.
                int fromStackParamBytesTotal = (fromConvention.Cleanup == StackCleanup.Caller) ? (numberOfParameters - fromConvention.SourceRegisters.Length) * 4 : 0;
                int toStackParamBytesTotal = (toConvention.Cleanup == StackCleanup.Callee) ? (numberOfParameters - toConvention.SourceRegisters.Length) * 4 : 0;
                int stackCleanupBytesTotal = fromStackParamBytesTotal + fromConvention.ReservedStackSpace;

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
                assemblyCode.Add(Utilities.TryAssembleRelativeCallMnemonics_WithUnknownSourceAddress(codeAddress, functionAddress, functionSize, false, out _));

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
                return buffer.Add(Utilities.Assembler.Assemble(assemblyCode.ToArray()), 1);
            });
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
