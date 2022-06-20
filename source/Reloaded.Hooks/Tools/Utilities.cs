﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Internal;
using Reloaded.Memory.Buffers;
using Reloaded.Memory.Buffers.Internal.Kernel32;
using SharpDisasm;
using static Reloaded.Hooks.Internal.Native.MEM_PROTECTION;

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

        private static string SetAddress(nuint address) => $"org {address}";

        /// <summary>
        /// Writes a pointer to a given target address in unmanaged, non-reclaimable memory.
        /// </summary>
        /// <param name="target">The target address/value the pointer is pointing to.</param>
        /// <returns>Address of the pointer.</returns>
        public static unsafe nuint WritePointer(nuint target)
        {
            var buffer = FindOrCreateBufferInRange(sizeof(nuint));
            return buffer.Add(ref target);
        }

        /// <summary>
        /// Assembles an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssembleAbsoluteJump(nuint target, bool is64bit) => Assembler.Assemble(new[]
        {
            Architecture(is64bit),
            GetAbsoluteJumpMnemonics(target, is64bit)
        });

        /// <summary>
        /// Assembles a push + return combination to a given target address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssemblePushReturn(nuint target, bool is64bit) => Assembler.Assemble(new[]
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
        /// Assembles a relative (to EIP/RIP) jump by a user specified offset.
        /// </summary>
        /// <param name="currentAddress">Address of the current instruction.</param>
        /// <param name="targetAddress">The address to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static byte[] AssembleRelativeJump(nuint currentAddress, nuint targetAddress, bool is64bit)
        {
            return Assembler.Assemble(new[]
            {
                Architecture(is64bit),
                SetAddress(currentAddress),
                is64bit ? $"jmp qword {targetAddress}" : $"jmp dword {targetAddress}"
            });
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetAbsoluteJumpMnemonics(nuint target, bool is64bit)
        {
            var buffer = FindOrCreateBufferInRange(IntPtr.Size, 1, UInt32.MaxValue);
            nuint functionPointer = buffer.Add(ref target);

            if (is64bit) return "jmp qword [qword " + functionPointer + "]";
            else         return "jmp dword [" + functionPointer + "]";
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute call to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetAbsoluteCallMnemonics(nuint target, bool is64bit)
        {
            var buffer = FindOrCreateBufferInRange(IntPtr.Size);
            nuint functionPointer = buffer.Add(ref target);

            if (is64bit) return "call qword [qword " + functionPointer + "]";
            else         return "call dword [" + functionPointer + "]";
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
            return GetAbsoluteJumpMnemonics(reverseWrapper.WrapperPointer.ToUnsigned(), IntPtr.Size == 8);
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
            return GetAbsoluteCallMnemonics(reverseWrapper.WrapperPointer.ToUnsigned(), IntPtr.Size == 8);
        }

        /// <summary>
        /// Gets the sequence of assembly instructions required to assemble an absolute jump to a user specified address.
        /// </summary>
        /// <param name="target">The target memory location to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        public static string GetPushReturnMnemonics(nuint target, bool is64bit) => $"push {target}\nret";

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
        /// <param name="jumpTarget">The address to jump to.</param>
        /// <param name="opcodes">Bytes representing existing assembly instructions.</param>
        /// <param name="is64bit">True for x64 else x86</param>
        /// <param name="targetAddress">[Optional] Target address within of which the wrapper should be placed in <paramref name="maxDisplacement"/> range.</param>
        /// <param name="maxDisplacement">Maximum distance from the <paramref name="targetAddress"/></param>
        public static nuint InsertJump(byte[] opcodes, bool is64bit, nuint jumpTarget, nuint targetAddress = 0, nint maxDisplacement = Int32.MaxValue)
        {
            List<byte> newBytes = opcodes.ToList();
            newBytes.AddRange(AssembleAbsoluteJump(jumpTarget, is64bit));
            byte[] newBytesArray = newBytes.ToArray();

            var minMax = GetRelativeJumpMinMax(targetAddress, maxDisplacement);
            var buffer = FindOrCreateBufferInRange(newBytesArray.Length, minMax.min, minMax.max);

            return buffer.Add(newBytesArray);
        }

        /// <summary>
        /// Creates instructions to jump to a specified address and then writes them to the buffer.
        /// </summary>
        /// <param name="targetPtr">The address to jump to.</param>
        /// <param name="is64Bit">Whether the jump is 64-bit or not.</param>
        /// <param name="extraBytes">Extra bytes to allocate after the jump.</param>
        /// <returns>Pointer to the code used to jump to said specified address.</returns>
        public static nuint CreateJump(nuint targetPtr, bool is64Bit, int extraBytes = 0)
        {
            int maxFunctionSize = 64 + extraBytes;
            var minMax = Utilities.GetRelativeJumpMinMax(targetPtr, Int32.MaxValue - maxFunctionSize);
            var buffer = Utilities.FindOrCreateBufferInRange(maxFunctionSize, minMax.min, minMax.max);

            // If 

            return buffer.ExecuteWithLock(() =>
            {
                // Align the code.
                buffer.SetAlignment(16);
                var codeAddress = buffer.Properties.WritePointer;
                var bytes = TryAssembleRelativeJumpArray(codeAddress, targetPtr, is64Bit, out _);
                var result = buffer.Add(bytes, 1);
                if (extraBytes > 0)
                    buffer.Add(extraBytes, 1);

                return result;
            });
        }

        /// <summary>
        /// Retrieves the length of the hook for trampoline, mid-function hooks etc.
        /// </summary>
        /// <param name="hookAddress">The address that is to be hooked.</param>
        /// <param name="hookLength">The minimum length of the hook, the length of our assembled bytes for the hook.</param>
        /// <param name="is64Bit">True if 64bit, else false.</param>
        public static int GetHookLength(nuint hookAddress, int hookLength, bool is64Bit)
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
        public static int GetHookLength(nuint hookAddress, int hookLength, ArchitectureMode architectureMode)
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
        /// Retrieves the number of parameters for a type that inherits from <see cref="IFuncPtr"/>.
        /// Otherwise defaults to checking by type, assuming the type is a <see cref="Delegate"/>
        /// </summary>
        /// <typeparam name="TFunction">Type that inherits from <see cref="IFuncPtr"/> or contains a field that inherits from <see cref="IFuncPtr"/>.</typeparam>
        public static int GetNumberofParameters<TFunction>()
        {
            if (TryGetIFuncPtrFromType<TFunction>(out IFuncPtr ptr))
                return ptr.NumberOfParameters;

            return GetNumberofParameters(typeof(TFunction));
        }


        /// <summary>
        /// Retrieves the number of parameters for a type that inherits from <see cref="IFuncPtr"/>.
        /// Otherwise defaults to checking by type, assuming the type is a <see cref="Delegate"/>
        /// Ignores float and double parameters.
        /// </summary>
        /// <typeparam name="TFunction">Type that inherits from <see cref="IFuncPtr"/> or contains a field that inherits from <see cref="IFuncPtr"/>.</typeparam>
        public static int GetNumberofParametersWithoutFloats<TFunction>()
        {
            if (TryGetIFuncPtrFromType<TFunction>(out IFuncPtr ptr))
                return ptr.NumberOfParametersWithoutFloats;

            return GetNumberofParametersWithoutFloats(typeof(TFunction));
        }

        /// <summary>
        /// Tries to instantiate <see cref="IFuncPtr"/> from a <typeparamref name="TType"/> or and of the type's fields.
        /// </summary>
        private static bool TryGetIFuncPtrFromType<TType>(out IFuncPtr value)
        {
            value = null;

            // Search type directly.
            if (typeof(IFuncPtr).IsAssignableFrom(typeof(TType)))
            {
                value = (IFuncPtr) Activator.CreateInstance(typeof(TType));
                return true;
            }

            // Search all fields
            foreach (var field in typeof(TType).GetFields())
            {
                if (typeof(IFuncPtr).IsAssignableFrom(field.FieldType))
                {
                    value = (IFuncPtr)Activator.CreateInstance(field.FieldType);
                    return true;
                }
            }

            return false;
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
        /// Checks whether a given address in memory can be read from.
        /// </summary>
        /// <param name="address">The target address.</param>
        internal static unsafe bool IsBadReadPtr(IntPtr address)
        {
            const Native.MEM_PROTECTION canReadMask = (PAGE_READONLY | PAGE_READWRITE | PAGE_WRITECOPY | PAGE_EXECUTE_READ | PAGE_EXECUTE_READWRITE | PAGE_EXECUTE_WRITECOPY);
            Native.MEMORY_BASIC_INFORMATION mbi = default;

            if (Native.VirtualQuery(address, ref mbi, (UIntPtr) sizeof(Native.MEMORY_BASIC_INFORMATION)) != IntPtr.Zero)
            {
                bool badPtr = (mbi.Protect & canReadMask) == 0;
                
                // Check the page is not a guard page
                if ((mbi.Protect & (PAGE_GUARD | PAGE_NOACCESS)) > 0) 
                    badPtr = true;

                return badPtr;
            }

            return true;
        }

        /// <summary>
        /// Fills a given array with <paramref name="value"/> until the array of bytes is as large as <paramref name="length"/>.
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
        /// Assembles a relative jump if the target is within range,
        /// else assembles an absolute jump.
        /// </summary>
        /// <param name="source">The source address to jump from.</param>
        /// <param name="target">The target address to jump to.</param>
        /// <param name="is64Bit">True if 64 bit, else false.</param>
        /// <param name="isRelative">True if the jump is relative, else false.</param>
        public static byte[] TryAssembleRelativeJumpArray(nuint source, nuint target, bool is64Bit, out bool isRelative)
        {
            var minMax = GetRelativeJumpMinMax(source);
            isRelative = new AddressRange(minMax.min, minMax.max).Contains(target);
            return isRelative ?
                AssembleRelativeJump(source, target, is64Bit) :
                AssembleAbsoluteJump(target, is64Bit);
        }

        /// <summary>
        /// Assembles a relative jump if the target is within range,
        /// else assembles an absolute jump.
        /// </summary>
        /// <param name="source">The source address to jump from.</param>
        /// <param name="target">The target address to jump to.</param>
        /// <param name="is64Bit">True if 64 bit, else false.</param>
        /// <param name="isRelative">True if the jump is relative, else false.</param>
        public static List<byte> TryAssembleRelativeJump(nuint source, nuint target, bool is64Bit, out bool isRelative)
        {
            return TryAssembleRelativeJumpArray(source, target, is64Bit, out isRelative).ToList();
        }

        /// <summary>
        /// Finds an existing <see cref="MemoryBuffer"/> or creates one satisfying the given size.
        /// </summary>
        /// <param name="size">The required size of buffer.</param>
        /// <param name="minimumAddress">Maximum address of the buffer.</param>
        /// <param name="maximumAddress">Minimum address of the buffer.</param>
        /// <param name="alignment">Required alignment of the item to add to the buffer.</param>
        public static MemoryBuffer FindOrCreateBufferInRange(int size, nuint minimumAddress = 1, nuint maximumAddress = int.MaxValue, int alignment = 4)
        {
            var buffers = _bufferHelper.FindBuffers(size + alignment, minimumAddress, maximumAddress);
            return buffers.Length > 0 ? buffers[0] : _bufferHelper.CreateMemoryBuffer(size, minimumAddress, maximumAddress);
        }

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
        public static (nuint min, nuint max) GetRelativeJumpMinMax(nuint targetAddress, nint maxDisplacement = Int32.MaxValue)
        {
            var minAddress = targetAddress - (nuint)maxDisplacement;
            if (minAddress > targetAddress) // Check for underflow.
                minAddress = 1;             // Limitation of Reloaded.Memory.Buffers

            var maxAddress = targetAddress + (nuint)maxDisplacement;
            if (maxAddress < targetAddress)
                maxAddress = unchecked((nuint)(-1)); // nuint.MaxValue not available in netstandard2.0

            if (!Environment.Is64BitProcess && maxAddress > UInt32.MaxValue)
                maxAddress = UInt32.MaxValue;

            return (minAddress, maxAddress);
        }
    }
}
