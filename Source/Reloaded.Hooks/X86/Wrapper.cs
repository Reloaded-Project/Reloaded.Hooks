using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;

namespace Reloaded.Hooks.X86
{
    /// <summary>
    /// The <see cref="Wrapper"/> is a marshaller which converts a CDECL function call
    /// to Custom Calling Convention call.
    /// This means that you can call Custom Calling Convention functions as if it was a CDECL function.
    /// </summary>
    public static class Wrapper
    {
        /// <summary>
        /// Creates the <see cref="Wrapper"/> which allows you to call a function with a custom calling
        /// convention as if it were a CDECL function.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        public static TFunction Create<TFunction>(long functionAddress)
        {
            return Create<TFunction>(functionAddress, out var wrapperAddress);
        }

        /// <summary>
        /// Creates the <see cref="Wrapper"/> which allows you to call a function with a custom calling
        /// convention as if it were a CDECL function.
        /// </summary>
        /// <param name="functionAddress">Address of the function to reverse wrap..</param>
        /// <param name="wrapperAddress">
        ///     Address of the wrapper used to call the original function.
        ///     If the original function is CDECL, the wrapper address equals the function address.
        /// </param>
        public static TFunction Create<TFunction>(long functionAddress, out IntPtr wrapperAddress)
        {
            var attribute = FunctionAttribute.GetAttribute<TFunction>();
            wrapperAddress = (IntPtr)functionAddress;

            // Hot path: CDECL functions require no wrapping.
            if (! attribute.Equals(new FunctionAttribute(CallingConventions.Cdecl)))
                wrapperAddress = Create<TFunction>((IntPtr)functionAddress, attribute);

            return Marshal.GetDelegateForFunctionPointer<TFunction>(wrapperAddress);
        }

        /// <summary>
        /// Creates the <see cref="Wrapper"/> in memory allowing you to call a function
        /// at functionAddress as if it was a CDECL function.
        /// </summary>
        /// <param name="functionAddress">The address of the function.</param>
        /// <param name="fromFunction">Describes the properties of the function to wrap.</param>
        /// <returns>Address of the wrapper in memory you can call like a CDECL function.</returns>
        public static IntPtr Create<TFunction>(IntPtr functionAddress, IFunctionAttribute fromFunction)
        {
            Mutex.MakeWrapperMutex.WaitOne();

            // toFunction (target) is CDECL
            int numberOfParameters = Utilities.GetNumberofParameters(typeof(TFunction));
            int nonRegisterParameters = numberOfParameters - fromFunction.SourceRegisters.Length;
            List<string> assemblyCode = new List<string> {"use32"};

            // Backup Stack Frame
            assemblyCode.Add("push ebp");       // Backup old call frame
            assemblyCode.Add("mov ebp, esp");   // Setup new call frame

            // Reserve Extra Stack Space
            if (fromFunction.ReservedStackSpace > 0)
                assemblyCode.Add($"sub esp, {fromFunction.ReservedStackSpace}");

            // Setup Function Parameters
            if (numberOfParameters > 0)
                assemblyCode.AddRange(AssembleFunctionParameters(numberOfParameters, fromFunction.SourceRegisters));

            // Call target function
            var pointerBuffer = Utilities.FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr targetFunctionPtr = pointerBuffer.Add(ref functionAddress);
            assemblyCode.Add("call dword [0x" + targetFunctionPtr.ToString("X") + "]"); 

            // Stack cleanup if necessary 
            if (nonRegisterParameters > 0 && fromFunction.Cleanup == FunctionAttribute.StackCleanup.Caller)
                assemblyCode.Add($"add esp, {nonRegisterParameters * 4}");

            // Setup return register
            if (fromFunction.ReturnRegister != FunctionAttribute.Register.eax)
                assemblyCode.Add("mov eax, " + fromFunction.ReturnRegister);

            // Unreserve Extra Stack Space
            if (fromFunction.ReservedStackSpace > 0)
                assemblyCode.Add($"add esp, {fromFunction.ReservedStackSpace}");

            // Restore Stack Frame and Return
            assemblyCode.Add("pop ebp");
            assemblyCode.Add("ret");
            
            // Write function to buffer and return pointer.
            byte[] assembledMnemonics = Utilities.Assembler.Assemble(assemblyCode.ToArray());
            var wrapperBuffer = Utilities.FindOrCreateBufferInRange(assembledMnemonics.Length);

            Mutex.MakeWrapperMutex.ReleaseMutex();
            return wrapperBuffer.Add(assembledMnemonics);
        }

        /// <summary>
        /// Generates the assembly code for passing to the wrapped
        /// custom calling convention function.
        /// </summary>
        /// <param name="parameterCount">The total amount of parameters that the target function accepts.</param>
        /// <param name="registers">The registers in left to right order to be passed onto the method.</param>
        /// <returns>A string array of compatible x86 mnemonics to be assembled.</returns>
        private static string[] AssembleFunctionParameters(int parameterCount, FunctionAttribute.Register[] registers)
        {
            List<string> assemblyCode = new List<string>();

            // At the current moment in time, the base address of old call stack (EBP) is at [ebp + 0]
            // the return address of the calling function is at [ebp + 4], last parameter is therefore at [ebp + 8].
            // Note: Reason return address is not at [ebp + 0] is because we pushed ebp and mov'd esp to it.
            // Reminder: The stack grows by DECREMENTING THE STACK POINTER.

            // The initial offset from EBP (Stack Base Pointer) for the rightmost parameter (right to left passing):
            int currentBaseStackOffset = ((parameterCount + 1) * 4);
            int nonRegisterParameters = parameterCount - registers.Length;
            
            // Re-push our non-register parameters passed onto the method onto the stack.
            // (Right to left order)
            for (int x = 0; x < nonRegisterParameters; x++)
            {
                assemblyCode.Add($"push dword [ebp + {currentBaseStackOffset}]");
                currentBaseStackOffset -= 4;
            }

            // Now move the remaining parameters into the target registers.
            // We reverse the left to right register order to right to left however.
            FunctionAttribute.Register[] newRegisters = registers.Reverse().ToArray();
            foreach (FunctionAttribute.Register registerParameter in newRegisters)
            {
                assemblyCode.Add($"mov {registerParameter}, [ebp + {currentBaseStackOffset}]");
                currentBaseStackOffset -= 4;
            }

            return assemblyCode.ToArray();
        }
    }
}
