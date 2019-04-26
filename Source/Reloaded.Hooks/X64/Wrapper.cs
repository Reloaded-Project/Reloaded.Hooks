using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;

namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// The <see cref="Wrapper"/> is a marshaller which converts a Microsoft x64 function call
    /// to Custom Calling Convention call.
    /// This means that you can call Custom Calling Convention functions as if it was a Microsoft x64 function.
    /// </summary>
    public static class Wrapper
    {
        /// <summary>
        /// Creates the <see cref="Wrapper"/> which allows you to call a function with a custom calling
        /// convention as if it were a Microsoft X64 calling convention function.
        /// </summary>
        /// <param name="functionAddress">Address of the function to wrap.</param>
        public static TFunction Create<TFunction>(long functionAddress)
        {
            var attribute = FunctionAttribute.GetAttribute<TFunction>();
            IntPtr wrapperFunctionPointer = (IntPtr)functionAddress;

            // Hot path: Microsoft X64 functions require no wrapping.
            if (!attribute.Equals(new FunctionAttribute(CallingConventions.Microsoft)))
                wrapperFunctionPointer = Create<TFunction>((IntPtr)functionAddress, attribute, new FunctionAttribute(CallingConventions.Microsoft));

            return Marshal.GetDelegateForFunctionPointer<TFunction>(wrapperFunctionPointer);
        }

        /// <summary>
        /// Creates a wrapper converting a call to a source calling convention to a given target calling convention.
        /// </summary>
        /// <param name="functionAddress">Address of the function in fromConvention to execute.</param>
        /// <param name="fromConvention">The calling convention to convert to toConvention. This is the convention of the function called.</param>
        /// <param name="toConvention">The target convention to which convert to fromConvention. This is the convention of the function returned.</param>
        /// <returns>Address of the wrapper in memory you can call .</returns>
        public static IntPtr Create<TFunction>(IntPtr functionAddress, FunctionAttribute fromConvention, FunctionAttribute toConvention)
        {
            Mutex.MakeWrapperMutex.WaitOne();

            // Retrieve number of parameters.
            int numberOfParameters    = Utilities.GetNumberofParametersWithoutFloats(typeof(TFunction));
            List<string> assemblyCode = new List<string> {"use64"};

            // Backup Stack Frame
            assemblyCode.Add("push rbp");       // Backup old call frame
            assemblyCode.Add("mov rbp, rsp");   // Setup new call frame

            // Even my mind gets a bit confused. So here is a reminder:
            // fromConvention is the convention that gets called.
            // toConvention is the convention we are marshalling from.
            
            int targetStackParameters  = numberOfParameters - fromConvention.SourceRegisters.Length;
            targetStackParameters      = targetStackParameters < 0 ? 0 : targetStackParameters;

            int stackParamBytesTotal = (targetStackParameters * 8);
            int stackMisalignment    = (stackParamBytesTotal) % 16;
            int shadowSpace          = fromConvention.ShadowSpace ? 32 : 0;

            // Note: Our stack is already aligned at this point because of `push` above.
            // stackBytesTotal % 16 represent the amount of bytes away from alignment after pushing parameters up the stack.
            // Setup stack alignment.
            if (stackMisalignment != 0)
                assemblyCode.Add($"sub rsp, {stackMisalignment}"); 

            // Setup parameters for target.
            if (numberOfParameters > 0)
                assemblyCode.AddRange(AssembleFunctionParameters(numberOfParameters, ref fromConvention, ref toConvention));

            // Make shadow space if target requires it.
            if (fromConvention.ShadowSpace)
                assemblyCode.Add($"sub rsp, {shadowSpace}");

            // Call target.
            var pointerBuffer = Utilities.FindOrCreateBufferInRange(IntPtr.Size, 1, UInt32.MaxValue);
            IntPtr targetFunctionPtr = pointerBuffer.Add(ref functionAddress);
            assemblyCode.Add("call qword [qword 0x" + targetFunctionPtr.ToString("X") + "]");

            // Restore the stack pointer after function call.
            if (stackParamBytesTotal + shadowSpace + stackMisalignment != 0)
                assemblyCode.Add($"add rsp, {stackParamBytesTotal + shadowSpace + stackMisalignment}" );

            // Marshal return register back from target to source.
            if (fromConvention.ReturnRegister != toConvention.ReturnRegister)
                assemblyCode.Add($"mov {fromConvention.ReturnRegister}, {toConvention.ReturnRegister}");

            // Restore Stack Frame and Return
            assemblyCode.Add("pop rbp");
            assemblyCode.Add("ret");

            // Write function to buffer and return pointer.
            byte[] assembledMnemonics = Utilities.Assembler.Assemble(assemblyCode.ToArray());
            var wrapperBuffer = Utilities.FindOrCreateBufferInRange(assembledMnemonics.Length, 1, long.MaxValue);

            Mutex.MakeWrapperMutex.ReleaseMutex();
            return wrapperBuffer.Add(assembledMnemonics);
        }

        private static string[] AssembleFunctionParameters(int parameterCount, ref FunctionAttribute fromConvention, ref FunctionAttribute toConvention)
        {
            List<string> assemblyCode = new List<string>();

            // At the current moment in time, our register contents and parameters are as follows: RCX, RDX, R8, R9.
            // The base address of old call stack (EBP) is at [ebp + 0]
            // The return address of the calling function is at [ebp + 8], last parameter is therefore at [ebp + 16].

            // Note: Reason return address is not at [ebp + 0] is because we pushed rbp and mov'd rsp to it.
            // Reminder: The stack grows by DECREMENTING THE STACK POINTER.
            
            int toStackParams     = parameterCount - toConvention.SourceRegisters.Length;
            int baseStackOffset   = toConvention.ShadowSpace ? 32 : 0;
            baseStackOffset       = (toStackParams + 1) * 8;

            // Re-push all source call convention stack params, then register parameters. (Right to Left)
            for (int x = 0; x < toStackParams; x++)
            {
                assemblyCode.Add($"push qword [rbp + {baseStackOffset}]");
                baseStackOffset -= 8;
            }
            
            for (int x = Math.Min(toConvention.SourceRegisters.Length, parameterCount) - 1; x >= 0; x--) 
                assemblyCode.Add($"push {toConvention.SourceRegisters[x].ToString()}");

            // Pop all necessary registers to target. (Left to Right)
            for (int x = 0; x < fromConvention.SourceRegisters.Length && x < parameterCount; x++)
                assemblyCode.Add($"pop {fromConvention.SourceRegisters[x].ToString()}");                

            return assemblyCode.ToArray();
        }
    }
}
