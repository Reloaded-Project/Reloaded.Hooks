using System;
using System.Collections.Generic;
using System.Diagnostics;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif
using System.IO;
using System.Linq;
using System.Reflection;
using Iced.Intel;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Structs;
using Reloaded.Hooks.Internal;
using Reloaded.Memory.Buffers;
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
        public static byte[] AssembleRelativeJump(nuint currentAddress, nuint targetAddress, bool is64bit) => AssembleRelativeJump(currentAddress, targetAddress, is64bit, out _);

        /// <summary>
        /// Assembles a relative (to EIP/RIP) jump by a user specified offset.
        /// </summary>
        /// <param name="currentAddress">Address of the current instruction.</param>
        /// <param name="targetAddress">The address to jump to.</param>
        /// <param name="is64bit">True to generate x64 code, else false (x86 code).</param>
        /// <param name="isProxied">
        ///     True if this relative jump is handled through a proxy, i.e. this jump jumps to another jump which jumps to the real target.
        /// </param>
        public static byte[] AssembleRelativeJump(nuint currentAddress, nuint targetAddress, bool is64bit, out bool isProxied)
        {
            long offset = (long)targetAddress - (long)currentAddress;
            isProxied = Math.Abs(offset) > Int32.MaxValue;
            if (!isProxied)
            {
                return Assembler.Assemble(new[]
                {
                    Architecture(is64bit),
                    SetAddress(currentAddress),
                    is64bit ? $"jmp qword {targetAddress}" : $"jmp dword {targetAddress}"
                });
            }

            // Hack: Work around invalid jumps.
            // There are legitimate possibilities of edge cases whereby it may not be possible to
            // jump from source to target, such as when there isn't sufficient memory.  
            // We're going to try hack past this with a simple hack for now, it's not perfect but
            // it should be good enough in the meantime.

            // Note: This code only handles signed cases in 64-bit due to length of long.
            // but given the address space of 64b, I don't consider this to be a limitation in my lifetime.

            // If we are exceeding the max jump range, try to
            // find a buffer within the range of currentaddress and
            // jump to it, then absolute jump from that one.
            var minMax = GetRelativeJumpMinMax(currentAddress);
            var buffer  = FindOrCreateBufferInRange(16, minMax.min, minMax.max); // No code alignment as this is edge case.
            var absoluteJumpAddress = buffer.Add(AssembleAbsoluteJump(targetAddress, is64bit));
            return Assembler.Assemble(new[]
            {
                Architecture(is64bit),
                SetAddress(currentAddress),
                is64bit ? $"jmp qword {absoluteJumpAddress}" : $"jmp dword {absoluteJumpAddress}"
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
            var buffer = FindOrCreateBufferInRange(IntPtr.Size, 1, UInt32.MaxValue);
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
        public static string GetAbsoluteJumpMnemonics<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate
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
        public static string GetAbsoluteCallMnemonics<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate
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
        /// <param name="minBytesUsed">Minimum number of bytes to use in the buffer.</param>
        /// <returns>Pointer to the code used to jump to said specified address.</returns>
        public static nuint CreateJump(nuint targetPtr, bool is64Bit, int minBytesUsed = 0)
        {
            const int alignment = 16;
            int maxFunctionSize = alignment + Constants.MaxAbsJmpSize + minBytesUsed;
            var minMax = GetRelativeJumpMinMax(targetPtr, Int32.MaxValue - maxFunctionSize);
            var buffer = FindOrCreateBufferInRange(maxFunctionSize, minMax.min, minMax.max);
            
            return buffer.ExecuteWithLock(() =>
            {
                // Align the code.
                buffer.SetAlignment(alignment);
                var codeAddress = buffer.Properties.WritePointer;
                var bytes = TryAssembleRelativeJumpArray(codeAddress, targetPtr, is64Bit, out _);
                var result = buffer.Add(bytes, 1);

                var bytesUsed  = buffer.Properties.WritePointer - codeAddress;
                var extraBytes = minBytesUsed - (int)bytesUsed;
                if (extraBytes > 0)
                    buffer.Add((int)extraBytes, 1);

                return result;
            });
        }
        
        /// <summary>
        /// Retrieves the length of the hook for trampoline, mid-function hooks etc.
        /// </summary>
        /// <param name="hookAddress">The address that is to be hooked.</param>
        /// <param name="hookLength">The minimum length of the hook, the length of our assembled bytes for the hook.</param>
        /// <param name="is64Bit">True to disasm as 64-bit, else uses 32.</param>
        public static unsafe int GetHookLength(nuint hookAddress, int hookLength, bool is64Bit)
        {
            /*
                This works by disassembling the bytes at the given address and iterating
                over each individual instruction up to the point where the total length of the
                disassembled exceeds the user set length of instructions to be assembled.
             */

            using var unmanagedMemStream = new UnmanagedMemoryStream((byte*)hookAddress, 128);
            var decoder = Decoder.Create(is64Bit ? 64 : 32, new StreamCodeReader(unmanagedMemStream));

            int completeHookLength = 0;
            while (completeHookLength < hookLength)
            {
                decoder.Decode(out var instruction);
                completeHookLength += instruction.Length;
            }

            return completeHookLength;
        }

        /// <summary>
        /// Retrieves the number of parameters for a specific delegate Type.
        /// </summary>
        /// <param name="delegateType">A Type extracted from a Delegate.</param>
        /// <returns>Number of parameters for the supplied delegate type.</returns>
        public static int GetNumberofParameters(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            Type delegateType)
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
        public static int GetNumberofParametersWithoutFloats(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
            Type delegateType)
        {
            MethodInfo method = delegateType.GetMethod("Invoke");
            return method != null ? GetNonFloatParameters(method) : 0;
        }

        /// <summary>
        /// Retrieves the number of parameters for a type that inherits from <see cref="IFuncPtr"/>.
        /// Otherwise defaults to checking by type, assuming the type is a <see cref="Delegate"/>
        /// </summary>
        /// <typeparam name="TFunction">Type that inherits from <see cref="IFuncPtr"/> or contains a field that inherits from <see cref="IFuncPtr"/>.</typeparam>
        public static int GetNumberofParameters<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>()
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
        public static int GetNumberofParametersWithoutFloats<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>()
        {
            if (TryGetIFuncPtrFromType<TFunction>(out IFuncPtr ptr))
                return ptr.NumberOfParametersWithoutFloats;

            return GetNumberofParametersWithoutFloats(typeof(TFunction));
        }

        /// <summary>
        /// Tries to instantiate <see cref="IFuncPtr"/> from a <typeparamref name="TType"/> or and of the type's fields.
        /// </summary>
        private static bool TryGetIFuncPtrFromType<
#if NET5_0_OR_GREATER
        [DynamicallyAccessedMembers(Trimming.FuncPtrTypes)]
#endif
        TType>(out IFuncPtr value)
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
                    value = (IFuncPtr)CreateInstanceSuppressed(field);
                    return true;
                }
            }

            return false;

#if NET5_0_OR_GREATER
            [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2072", Justification = "Nested parameterless constructor preserved via nested types.")]
#endif
            static object CreateInstanceSuppressed(FieldInfo field) => Activator.CreateInstance(field.FieldType);
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

        // Note: Internal because I don't yet know if I want to expose this.

        /// <summary>
        /// Gets mnemonics for a relative call if the target is within range,
        /// else gets mnemonics for an absolute call. This function can be used when exact address
        /// of 'source' is unknown; (i.e. included as part of some assembler generated code).
        /// </summary>
        /// <param name="source">The source address to start of assembled instructions.</param>
        /// <param name="target">The target address to jump to.</param>
        /// <param name="minCodeSize">Minimum size of code assembled at <paramref name="source"/>.</param>
        /// <param name="is64Bit">True if 64 bit, else false.</param>
        /// <param name="isRelative">True if the call is relative, else false.</param>
        internal static string TryAssembleRelativeCallMnemonics_WithUnknownSourceAddress(nuint source, nuint target, int minCodeSize, bool is64Bit, out bool isRelative)
        {
            // If call is `20 bytes` after `source`, and target is behind source, we need 20 more bytes; hence our use of minCodeSize is ok.
            // If call is `20 bytes` after `source`, and target is after source, we are 20 bytes closer and are also ok.
            var minMax = GetRelativeJumpMinMax(source, Int32.MaxValue - minCodeSize);
            isRelative = new AddressRange(minMax.min, minMax.max).Contains(target);
            return isRelative ?
                is64Bit ? $"call qword {target}" : $"call dword {target}" :
                GetAbsoluteCallMnemonics(target, is64Bit);
        }

        /// <summary>
        /// Finds an existing <see cref="MemoryBuffer"/> or creates one satisfying the given size.
        /// </summary>
        /// <param name="size">The required size of buffer.</param>
        /// <param name="minimumAddress">Maximum address of the buffer.</param>
        /// <param name="maximumAddress">Minimum address of the buffer.</param>
        /// <param name="alignment">Required alignment of the item to add to the buffer.</param>
        /// <exception cref="Exception">Unable to find or create a buffer in given range.</exception>
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
