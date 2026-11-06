using System;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Benchmarks
{
    public class NativeCalculator64 : NativeCalculator, IDisposable
    {
        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        private Reloaded.Memory.Memory _memory = Reloaded.Memory.Memory.Instance;

        public NativeCalculator64()
        {
            BuildAdd();
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            _memory.Free(new Reloaded.Memory.Structs.MemoryAllocation(Add, (nuint)0));
            _assembler.Dispose();
        }

        private void BuildAdd()
        {
            string[] addFunction = new string[]
            {
                $"use64",
                $"lea eax, [rcx+rdx]",
                $"ret",
            };

            var result = _assembler.Assemble(addFunction);
            Add = _memory.Allocate((nuint)result.Length).Address;
            _memory.WriteRaw(Add, result);
        }

    }
}
