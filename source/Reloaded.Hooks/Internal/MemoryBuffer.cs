using System;
using Reloaded.Memory.Buffers.Structs;

namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// A linearly written, executable (RWX) memory buffer backed by a private allocation
    /// made through <see cref="Reloaded.Memory.Buffers"/>.
    /// </summary>
    public class MemoryBuffer
    {
        // Kept alive for the lifetime of the buffer; the allocation is freed by the
        // garbage collector once this reference is dropped, which would invalidate generated code.
        private readonly PrivateAllocation _allocation;
        private readonly object _lock = new object();

        /// <summary>
        /// Base address of the underlying allocation.
        /// </summary>
        public nuint BaseAddress { get; }

        /// <summary>
        /// Address one past the end of the underlying allocation.
        /// </summary>
        public nuint EndAddress { get; }

        /// <summary>
        /// Address at which the next item will be written.
        /// </summary>
        public nuint WritePointer { get; private set; }

        /// <summary>
        /// Exposes the current state of the buffer.
        /// </summary>
        public MemoryBufferProperties Properties => new MemoryBufferProperties(WritePointer);

        public MemoryBuffer(PrivateAllocation allocation)
        {
            _allocation = allocation;
            BaseAddress = allocation.BaseAddress;
            EndAddress = allocation.BaseAddress + allocation.Size;
            WritePointer = BaseAddress;
        }

        /// <summary>
        /// Advances the write pointer to the next boundary defined by <paramref name="alignment"/>.
        /// </summary>
        public void SetAlignment(int alignment)
        {
            if (alignment <= 1)
                return;

            var remainder = WritePointer % (nuint)alignment;
            if (remainder != 0)
                WritePointer += (nuint)alignment - remainder;
        }

        /// <summary>
        /// Writes an array of bytes to the buffer and returns the address they were written to.
        /// </summary>
        public unsafe nuint Add(byte[] bytesToWrite, int alignment = 4)
        {
            SetAlignment(alignment);
            var address = WritePointer;
            var destination = new Span<byte>((void*)address, bytesToWrite.Length);
            bytesToWrite.AsSpan().CopyTo(destination);
            WritePointer += (nuint)bytesToWrite.Length;
            return address;
        }

        /// <summary>
        /// Reserves a given amount of bytes in the buffer and returns the address of the reserved region.
        /// </summary>
        public nuint Add(int numBytes, int alignment)
        {
            SetAlignment(alignment);
            var address = WritePointer;
            WritePointer += (nuint)numBytes;
            return address;
        }

        /// <summary>
        /// Writes an unmanaged value to the buffer and returns the address it was written to.
        /// </summary>
        public unsafe nuint Add<T>(ref T item, int alignment = 4) where T : unmanaged
        {
            SetAlignment(alignment);
            var address = WritePointer;
            *(T*)address = item;
            WritePointer += (nuint)sizeof(T);
            return address;
        }

        /// <summary>
        /// Writes an unmanaged value to the buffer and returns the address it was written to.
        /// </summary>
        /// <remarks>
        /// The <paramref name="marshalElement"/> parameter is retained for source compatibility and is ignored;
        /// only unmanaged (non-marshalled) writes are supported by this buffer.
        /// </remarks>
        public nuint Add<T>(ref T item, bool marshalElement, int alignment = 4) where T : unmanaged
            => Add(ref item, alignment);

        /// <summary>
        /// Executes the supplied function while holding the buffer's lock.
        /// </summary>
        public T ExecuteWithLock<T>(Func<T> code)
        {
            lock (_lock)
                return code();
        }

        /// <summary>
        /// Returns true if an item of the given size can be written to the buffer within the specified address range.
        /// </summary>
        public bool CanFit(int size, nuint minimumAddress, nuint maximumAddress)
        {
            var start = WritePointer;
            var end = start + (nuint)size;

            return start >= minimumAddress &&
                   end <= maximumAddress &&
                   end <= EndAddress;
        }
    }

    /// <summary>
    /// Describes the current write state of a <see cref="MemoryBuffer"/>.
    /// </summary>
    public readonly struct MemoryBufferProperties
    {
        /// <summary>
        /// Address at which the next item will be written.
        /// </summary>
        public nuint WritePointer { get; }

        public MemoryBufferProperties(nuint writePointer) => WritePointer = writePointer;
    }
}
