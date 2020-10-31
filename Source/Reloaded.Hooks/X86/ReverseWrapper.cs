using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;

namespace Reloaded.Hooks.X86
{
    /// <summary>
    /// The <see cref="ReverseWrapper{TFunction}"/> is a marshaller which converts a Custom Convention function call
    /// to CDECL call.
    /// This means that you can call CDECL functions as if it they were Custom Convention calls.
    /// </summary>
    public class ReverseWrapper<TFunction> : IReverseWrapper<TFunction>
    {
        /// <summary> Copy of C# function behind the pointer. </summary>
        public TFunction CSharpFunction { get; }

        /// <summary> Pointer to the function that gets executed inside the wrapper, either native or C#. </summary>
        public IntPtr NativeFunctionPtr { get; }

        /// <summary> A pointer to our wrapper, which calls the internal method as if it were to be of another convention. </summary>
        public IntPtr WrapperPointer { get; private set; }

        /// <summary>
        /// Creates the <see cref="ReverseWrapper{TFunction}"/> which allows you to call
        /// a CDECL C# function, via a pointer as if it was a function of another calling convention.
        /// </summary>
        /// <remarks>
        ///     Please keep a reference to this class as long as you are using it.
        ///     Otherwise Garbage Collection will break the native function pointer to your C# function
        ///     resulting in a spectacular crash if it is still used anywhere.
        /// </remarks>
        /// <param name="function">The function to create a pointer to.</param>
        public ReverseWrapper(TFunction function)
        {
            CSharpFunction = function;
            NativeFunctionPtr = Marshal.GetFunctionPointerForDelegate(function);
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr);
        }

        /// <summary>
        /// Creates the <see cref="ReverseWrapper{TFunction}"/> which allows you to call
        /// a native CDECL function, via a pointer as if it was a function of another calling convention.
        /// </summary>
        /// <param name="function">Pointer of native function to wrap.</param>
        public ReverseWrapper(IntPtr function)
        {
            NativeFunctionPtr = function;
            WrapperPointer = NativeFunctionPtr;

            // Call above may or may not replace WrapperPointer.
            Create(this, NativeFunctionPtr);
        }

        private static void Create(ReverseWrapper<TFunction> reverseFunctionWrapper, IntPtr functionPtr)
        {
            var reloadedFunctionAttribute = FunctionAttribute.GetAttribute<TFunction>();

            // CDECL is hot path, as our TFunction will already be CDECL, we marshal if it's anything else.
            if (! reloadedFunctionAttribute.Equals(FunctionAttribute.Cdecl))
                reverseFunctionWrapper.WrapperPointer = Create(functionPtr, reloadedFunctionAttribute);
        }
        
        private static IntPtr Create(IntPtr functionAddress, IFunctionAttribute fromFunction)
        {
            Mutex.MakeReverseWrapperMutex.WaitOne();

            // Retrieve number of parameters and setup list of ASM instructions to be compiled.
            int numberOfParameters    = Utilities.GetNumberofParameters(typeof(TFunction));
            int nonRegisterParameters = numberOfParameters - fromFunction.SourceRegisters.Length;
            List<string> assemblyCode = new List<string> { "use32" };

            // Backup Stack Frame
            assemblyCode.Add("push ebp");       // Backup old call frame
            assemblyCode.Add("mov ebp, esp");   // Setup new call frame

            // Push registers for our C# method as necessary.
            assemblyCode.AddRange(AssembleFunctionParameters(numberOfParameters, fromFunction.SourceRegisters));

            // Call target function
            var pointerBuffer = Utilities.FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr targetFunctionPtr = pointerBuffer.Add(ref functionAddress);
            assemblyCode.Add("call dword [0x" + targetFunctionPtr.ToString("X") + "]");

            // MOV EAX return register into custom calling convention's return register.
            if (fromFunction.ReturnRegister != FunctionAttribute.Register.eax)
                assemblyCode.Add($"mov {fromFunction.ReturnRegister}, eax");

            // Restore stack pointer.
            if (numberOfParameters > 0)
                assemblyCode.Add($"add esp, {numberOfParameters * 4}");

            // Restore Stack Frame and Return
            assemblyCode.Add("pop ebp");

            if (fromFunction.Cleanup == FunctionAttribute.StackCleanup.Callee)
                assemblyCode.Add($"ret {nonRegisterParameters * 4}");
            else
                assemblyCode.Add("ret");

            byte[] assembledMnemonics = Utilities.Assembler.Assemble(assemblyCode.ToArray());
            var wrapperBuffer = Utilities.FindOrCreateBufferInRange(assembledMnemonics.Length);
            var result = wrapperBuffer.Add(assembledMnemonics);

            Mutex.MakeReverseWrapperMutex.ReleaseMutex();
            return result;
        }

        /// <summary>
        /// Generates the assembly code for passing to the reverse wrapped
        /// CDECL compliant function.
        /// </summary>
        /// <param name="parameterCount">The total amount of parameters that the target function accepts.</param>
        /// <param name="registers">The registers in left to right order used to pass parameters.</param>
        /// <returns>A string array of compatible x86 mnemonics to be assembled.</returns>
        private static string[] AssembleFunctionParameters(int parameterCount, FunctionAttribute.Register[] registers)
        {
            List<string> assemblyCode = new List<string>();

            // At the current moment in time, the base address of old call stack (EBP) is at [ebp + 0].
            // The return address of the calling function is at [ebp + 4], last parameter is therefore at [ebp + 8].
            // Note: Reason return address is not at [ebp + 0] is because we pushed ebp and mov'd esp to it.
            // Reminder: The stack grows by DECREMENTING THE STACK POINTER.

            // The initial offset from EBP (Stack Base Pointer) for the rightmost parameter (right to left passing):
            int nonRegisterParameters = parameterCount - registers.Length;
            int currentBaseStackOffset = ((nonRegisterParameters + 1) * 4);

            // Re-push our non-register parameters passed onto the method onto the stack.
            // This happens in right to left order.
            for (int x = 0; x < nonRegisterParameters; x++)
            {
                // Push parameter onto stack and go to next parameter.
                assemblyCode.Add($"push dword [ebp + {currentBaseStackOffset}]");
                currentBaseStackOffset -= 4;
            }

            // Now push the remaining parameters from the custom calling convention's registers onto the stack.
            // We push in right to left order to comply with CDECL standards.
            FunctionAttribute.Register[] newRegisters = registers.Reverse().ToArray();
            foreach (FunctionAttribute.Register registerParameter in newRegisters)
                assemblyCode.Add($"push {registerParameter.ToString()}");

            return assemblyCode.ToArray();
        }
    }
}
