using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Reloaded.Memory.Buffers;
using SharpDisasm;

namespace Reloaded.Hooks.Tools
{
    public static class Utilities
    {
        /// <summary>
        /// Assembler is costly to instantiate.
        /// We statically instantiate it here to avoid multiple instantiations.
        /// </summary>
        public static Assembler.Assembler Assembler { get; }
        private static MemoryBufferHelper _bufferHelper;

        static Utilities()
        {
            Assembler     = new Assembler.Assembler();
            _bufferHelper = new MemoryBufferHelper(Process.GetCurrentProcess());
        }

        private static string Architecture(bool is64bit) => is64bit ? "use64" : "use32";

        /// <summary>
        /// Assembles an absolute jump to a user specified address.
        /// </summary>
        public static byte[] AssembleAbsoluteJump(IntPtr functionAddress, bool is64bit)
        {
            List<string> assemblyCode = new List<string>(2) { Architecture(is64bit) };

            var buffer              = FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr functionPointer  = buffer.Add(ref functionAddress);

            if (is64bit) assemblyCode.Add("jmp qword [qword 0x" + functionPointer.ToString("X") + "]");
            else         assemblyCode.Add("jmp dword [0x" + functionPointer.ToString("X") + "]");

            return Assembler.Assemble(assemblyCode);
        }


        /// <summary>
        /// Assembles a push + return combination to a given target address.
        /// </summary>
        /// <param name="targetAddress">A 32bit target address.</param>
        /// <param name="is64bit">32bit or 64bit process?</param>
        /// <returns></returns>
        public static byte[] AssemblePushReturn(IntPtr targetAddress, bool is64bit)
        {
            List<string> assemblyCode = new List<string>(2) { Architecture(is64bit) };

            assemblyCode.Add($"push 0x{targetAddress.ToString("X")}");
            assemblyCode.Add("ret");
            
            return Assembler.Assemble(assemblyCode);
        }

        /// <summary>
        /// Assembles a relative jump to by a user specified offset and returns
        /// the resultant bytes of the assembly process.
        /// </summary>
        public static byte[] AssembleRelativeJump(IntPtr relativeJumpOffset, bool is64bit)
        {
            List<string> assemblyCode = new List<string> { Architecture(is64bit) };

            if (is64bit) assemblyCode.Add("jmp qword " + relativeJumpOffset);
            else         assemblyCode.Add("jmp dword " + relativeJumpOffset);

            return Assembler.Assemble(assemblyCode.ToArray());
        }

        /// <summary>
        /// Appends an absolute jump to the supplied opcodes and assembles the result, returning a pointer.
        /// </summary>
        /// <param name="targetAddress">[Optional] Target address within of which the wrapper should be placed in 2GB range.</param>
        public static IntPtr InsertJump(byte[] opcodes, bool is64bit, long jumpTarget, long targetAddress = 0, long maxDisplacement = Int32.MaxValue)
        {
            List<byte> newBytes = opcodes.ToList();
            newBytes.AddRange(AssembleAbsoluteJump((IntPtr)jumpTarget, is64bit));
            byte[] newBytesArray = newBytes.ToArray();

            var minMax = GetRelativeJumpMinMax(targetAddress, maxDisplacement);
            var buffer = FindOrCreateBufferInRange(newBytesArray.Length, minMax.min, minMax.max);

            return buffer.Add(newBytesArray);
        }

        /// <summary>
        /// Retrieves the length of the hook for trampoline, mid-function hooks etc.
        /// </summary>
        /// <param name="hookAddress">The address that is to be hooked.</param>
        /// <param name="hookLength">The minimum length of the hook, the length of our assembled bytes for the hook.</param>
        /// <param name="is64Bit">True if 64bit, else false.</param>
        public static int GetHookLength(IntPtr hookAddress, int hookLength, bool is64Bit)
        {
            ArchitectureMode architecture = is64Bit ? ArchitectureMode.x86_64 
                                                    : ArchitectureMode.x86_32;
            return GetHookLength(hookAddress, hookLength, architecture);
        }

        /// <summary>
        /// Retrieves the length of the hook for trampoline, mid-function hooks etc.
        /// </summary>
        /// <param name="hookAddress">The address that is to be hooked.</param>
        /// <param name="hookLength">The minimum length of the hook, the length of our assembled bytes for the hook.</param>
        /// <param name="architectureMode">X86 or X64 to use for disassembly.</param>
        public static int GetHookLength(IntPtr hookAddress, int hookLength, ArchitectureMode architectureMode)
        {
            /*
                This works by reading a short fixed array of bytes from memory then disassembling the bytes
                and iterating over each individual instruction up to the point where the total length of the
                disassembled exceeds the user set length of instructions to be assembled.
             */

            // Retrieve the function header, arbitrary length of <see below> bytes is used for this operation.
            // While you can technically build infinite length X86 instructions, anything greater than 16 to compare seems reasonable.
            Memory.Sources.Memory.CurrentProcess.ReadRaw(hookAddress, out byte[] functionHeader, 64);

            Disassembler disassembler = new Disassembler(functionHeader, architectureMode);
            Instruction[] instructions = disassembler.Disassemble().ToArray();

            int completeHookLength = 0;
            foreach (Instruction instruction in instructions)
            {
                completeHookLength += instruction.Length;
                if (completeHookLength >= hookLength)
                    break;
            }

            return completeHookLength;
        }

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate Type.
        /// </summary>
        /// <param name="delegateType">A Type extracted from a Delegate.</param>
        /// <returns>Number of parameters for the supplied delegate type.</returns>
        public static int GetNumberofParameters(Type delegateType)
        {
            MethodInfo method = delegateType.GetMethod("Invoke");
            return method != null ? method.GetParameters().Length : 0;
        }

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate Type,
        /// ignoring any floating point parameters.
        /// </summary>
        /// <param name="delegateType">A Type extracted from a Delegate.</param>
        /// <returns>Number of parameters for the supplied delegate type, without floats.</returns>
        public static int GetNumberofParametersWithoutFloats(Type delegateType)
        {
            MethodInfo method = delegateType.GetMethod("Invoke");
            return method != null ? GetNonFloatParameters(method) : 0;
        }

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate type minus the floating point parameters.
        /// </summary>
        /// <param name="methodInformation">Defines the individual information that describes a method to be called.</param>
        /// <returns>The number of non-float parameters.</returns>
        private static int GetNonFloatParameters(MethodInfo methodInformation)
        {
            // Retrieve all parameters.
            ParameterInfo[] parameters = methodInformation.GetParameters();

            // Check for non-float and return amount.
            return parameters.Count(parameter => parameter.ParameterType != typeof(Single) && 
                                                 parameter.ParameterType != typeof(Double));
        }

        /// <summary>
        /// Fills a given array with <see cref="value"/> until the array of bytes is as large as <see cref="length"/>.
        /// </summary>
        public static void FillArrayUntilSize<T>(List<T> array, T value, int length)
        {
            if (length > array.Count)
            {
                int values = length - array.Count;

                for (int x = 0; x < values; x++)
                    array.Add(value);
            }
        }

        public static MemoryBuffer FindOrCreateBufferInRange(int size, long minimumAddress = 1, long maximumAddress = int.MaxValue)
        {
            var buffers = _bufferHelper.FindBuffers(size, (IntPtr)minimumAddress, (IntPtr)maximumAddress);
            return buffers.Length > 0 ? buffers[0] : _bufferHelper.CreateMemoryBuffer(size, minimumAddress, maximumAddress);
        }

        /*
         * -----------------
         * Private Functions
         * -----------------
         */

        public static (long min, long max) GetRelativeJumpMinMax(long targetAddress, long maxDisplacement = Int32.MaxValue)
        {
            long minAddress = targetAddress - maxDisplacement;
            long maxAddress;

            // long overflow check.
            if (long.MaxValue - maxDisplacement < targetAddress)
                maxAddress = long.MaxValue;
            else
                maxAddress = targetAddress + maxDisplacement;
            
            if (minAddress <= 0)
                minAddress = 1;     // Limitation of Reloaded.Memory.Buffers

            if (! Environment.Is64BitProcess)
                if (maxAddress > Int32.MaxValue)
                    maxAddress = Int32.MaxValue;

            return (minAddress, maxAddress);
        }
    }
}
