using System;
using System.Collections.Generic;
using System.Diagnostics;
using Iced.Intel;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Buffers;

namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Implement a second pass to internal function patching via iced.
    /// Second pass catches regular application code. My function patcher is only for patching existing hooks and their return addresses.
    /// Iced handles the rest, such as patching short jumps to elsewhere in same function, call instructions etc.
    /// </summary>
    public class IcedPatcher
    {
        private static MemoryBufferHelper _helper = new MemoryBufferHelper(Process.GetCurrentProcess());

        // In
        private int _bitness;
        private byte[] _bytes;
        private IntPtr _originalFunctionAddress;

        // Out
        private IntPtr _newPrologueAddress;

        /// <summary>
        /// Creates a patcher, which patches an existing function prologue using Iced.
        /// </summary>
        /// <param name="is64Bit">Set to true if the prologue is 64bit code, else false.</param>
        /// <param name="bytes">The bytes of the prologue of the hooked function.</param>
        /// <param name="originalFunctionAddress">Address of the original function being patched by iced.</param>
        public IcedPatcher(bool is64Bit, byte[] bytes, IntPtr originalFunctionAddress)
        {
            _bitness = is64Bit ? 64 : 32;
            _bytes   = bytes;
            _originalFunctionAddress = originalFunctionAddress;
        }

        /// <summary>
        /// Encodes the original bytes for a new address fixing e.g. branches, calls, jumps for execution at a given address.
        /// </summary>
        /// <param name="newAddress">The new address to encode the original instructions for.</param>
        public byte[] EncodeForNewAddress(IntPtr newAddress)
        {
            var writer = new CodeWriterImpl(_bytes.Length * 2);
            var block  = new InstructionBlock(writer, DecodePrologue(), (ulong)newAddress);
            if (!BlockEncoder.TryEncode(_bitness, block, out var error, out _))
            {
                throw new Exception($"Reloaded Hooks: Internal Error in {nameof(Reloaded.Hooks.Internal)}/{nameof(IcedPatcher)}. " +
                                    $"Failed to re-encode code for new address. Process will probably die." +
                                    $"Error: {error}");
            }

            return writer.ToArray();
        }

        /// <summary>
        /// Patches the prologue of a function with iced, writes to <see cref="MemoryBuffer"/>
        /// and returns the new address of the prologue.
        /// </summary>
        /// <param name="jumpTarget">Appends a relative jump to this address at the end of the patched code. Set null to not add a jump.</param>
        /// <returns>
        ///     Address of the patched function added to a <see cref="MemoryBuffer"/>.
        ///     If this function has already been executed, returns the address of previously patched function.
        /// </returns>
        public IntPtr ToMemoryBuffer(IntPtr? jumpTarget)
        {
            if (_newPrologueAddress != IntPtr.Zero)
                return _newPrologueAddress;

            int alignment       = 16;
            var estimateLength  = (_bytes.Length * 2) + alignment; // Super generous! Exact length not known till relocated, just ensuring the size is enough under any circumstance.
            var minMax          = Utilities.GetRelativeJumpMinMax(jumpTarget.HasValue ? (long) jumpTarget.Value : 0, Int32.MaxValue - estimateLength);
            var buffer          = Utilities.FindOrCreateBufferInRange(estimateLength, minMax.min, minMax.max, alignment);
            return buffer.ExecuteWithLock(() =>
            {
                // Patch prologue
                buffer.SetAlignment(alignment);
                var newBaseAddress = buffer.Properties.WritePointer;
                var data           = EncodeForNewAddress(newBaseAddress);
                _newPrologueAddress = buffer.Add(data, 1);
                if (jumpTarget != null)
                    buffer.Add(Utilities.AssembleRelativeJump(buffer.Properties.WritePointer, jumpTarget.Value, _bitness == 64), 1);

                return _newPrologueAddress;
            });
        }

        /// <summary>
        /// Returns a set of instructions represented by the prologue this type was instantiated with.
        /// </summary>
        private IList<Instruction> DecodePrologue()
        {
            var codeReader = new ByteArrayCodeReader(_bytes);
            var decoder    = Iced.Intel.Decoder.Create(_bitness, codeReader);
            decoder.IP     = (ulong) _originalFunctionAddress;
            ulong endRip   = decoder.IP + (uint)_bytes.Length;

            var instructions = new InstructionList();
            while (decoder.IP < endRip)
                decoder.Decode(out instructions.AllocUninitializedElement());

            return instructions;
        }
        
        /// <summary>
        /// Simple and inefficient code writer that stores data in a List(byte) with a ToArray method to get data.
        /// </summary>
        private sealed class CodeWriterImpl : CodeWriter
        {
            private List<byte> _allBytes;
            public override void WriteByte(byte value) => _allBytes.Add(value);
            public byte[] ToArray() => _allBytes.ToArray();

            public CodeWriterImpl(int capacity)
            {
                _allBytes = new List<byte>(capacity);
            }
        }


    }
}
