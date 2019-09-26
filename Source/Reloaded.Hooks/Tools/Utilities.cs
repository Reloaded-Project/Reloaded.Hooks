using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Reloaded.Hooks.Definitions;
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

        private static object _lock = new object();
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
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssembleAbsoluteJump(IntPtr target, bool is64bit) => Assembler.Assemble(new[]
        {
            Architecture(is64bit),
            GetAbsoluteJumpMnemonics(target, is64bit)
        });

        /// <summary>
        /// Assembles a push + return combination to a given target address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssemblePushReturn(IntPtr target, bool is64bit) => Assembler.Assemble(new[]
        {
            Architecture(is64bit),
            GetPushReturnMnemonics(target, is64bit)
        });

        /// <summary>
        /// Assembles a relative (to EIP/RIP) jump by a user specified offset.
        /// </summary>
        /// <param name="relativeJumpOffset">Offset relative to EIP/RIP to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssembleRelativeJump(IntPtr relativeJumpOffset, bool is64bit) => Assembler.Assemble(new[]
        {
            Architecture(is64bit),
            GetRelativeJumpMnemonics(relativeJumpOffset, is64bit)
        });

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetAbsoluteJumpMnemonics(IntPtr target, bool is64bit)
        {
            var buffer = FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr functionPointer = buffer.Add(ref target);

            if (is64bit) return "jmp qword [qword 0x" + functionPointer.ToString("X") + "]";
            else         return "jmp dword [0x" + functionPointer.ToString("X") + "]";
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute call to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetAbsoluteCallMnemonics(IntPtr target, bool is64bit)
        {
            var buffer = FindOrCreateBufferInRange(IntPtr.Size);
            IntPtr functionPointer = buffer.Add(ref target);

            if (is64bit) return "call qword [qword 0x" + functionPointer.ToString("X") + "]";
            else         return "call dword [0x" + functionPointer.ToString("X") + "]";
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a C# function address.
        /// </summary>
        /// <param name="function">The C# function to create a jump to.</param>
        /// <param name="reverseWrapper">
        ///     The native reverse wrapper used to call your function.
        ///     Please keep a reference to this class as long as you are using the generated code.
        ///     i.e. make it a class/struct member on heap.
        /// </param>
        public static string GetAbsoluteJumpMnemonics<TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate
        {
            var hooks = ReloadedHooks.Instance;
            reverseWrapper = hooks.CreateReverseWrapper<TFunction>(function);
            return GetAbsoluteJumpMnemonics(reverseWrapper.WrapperPointer, IntPtr.Size == 8);
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute call to a C# function address.
        /// </summary>
        /// <param name="function">The C# function to create a jump to.</param>
        /// <param name="reverseWrapper">
        ///     The native reverse wrapper used to call your function.
        ///     Please keep a reference to this class as long as you are using the generated code.
        ///     i.e. make it a class/struct member on heap.
        /// </param>
        public static string GetAbsoluteCallMnemonics<TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate
        {
            var hooks = ReloadedHooks.Instance;
            reverseWrapper = hooks.CreateReverseWrapper<TFunction>(function);
            return GetAbsoluteCallMnemonics(reverseWrapper.WrapperPointer, IntPtr.Size == 8);
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetPushReturnMnemonics(IntPtr target, bool is64bit) => $"push 0x{target.ToString("X")}\nret";

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble a relative jump to the current instruction pointer.
        /// </summary>
        /// <param name="relativeJumpOffset">Offset relative to EIP/RIP to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetRelativeJumpMnemonics(IntPtr relativeJumpOffset, bool is64bit)
        {
            if (is64bit) return "jmp qword " + relativeJumpOffset;
            else         return "jmp dword " + relativeJumpOffset;
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

        /// <summary>
        /// Finds an existing <see cref="MemoryBuffer"/> or creates one satisfying the given size.
        /// </summary>
        /// <param name="size">The required size of buffer.</param>
        /// <param name="minimumAddress">Maximum address of the buffer.</param>
        /// <param name="maximumAddress">Minimum address of the buffer.</param>
        /// <param name="alignment">Required alignment of the item to add to the buffer.</param>
        public static MemoryBuffer FindOrCreateBufferInRange(int size, long minimumAddress = 1, long maximumAddress = int.MaxValue, int alignment = 4)
        {
            var buffers = _bufferHelper.FindBuffers(size + alignment, (IntPtr)minimumAddress, (IntPtr)maximumAddress);
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
