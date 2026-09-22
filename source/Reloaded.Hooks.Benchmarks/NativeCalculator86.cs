using System;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Benchmarks
{
    public class NativeCalculator86 : NativeCalculator, IDisposable
    {
        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        private Reloaded.Memory.Memory _memory = Reloaded.Memory.Memory.Instance;

        public NativeCalculator86()
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
                $"use32",
                $"mov eax, [esp+8]", // Right Parameter
                $"add eax, [esp+4]", // Left Parameter
                $"ret 8",
            };

            var result = _assembler.Assemble(addFunction);
            Add = _memory.Allocate((nuint)result.Length).Address;
            _memory.WriteRaw(Add, result);
        }
    }
}
