using System;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Benchmarks
{
    public class NativeCalculator86 : NativeCalculator, IDisposable
    {
        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        private Memory.Sources.Memory _memory  = new Memory.Sources.Memory();

        public NativeCalculator86()
        {
            BuildAdd();
        }

        [ExcludeFromCodeCoverage]
        public void Dispose()
        {
            _memory.Free(Add);
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
            Add = _memory.Allocate(result.Length);
            _memory.WriteRaw(Add, result);
        }
    }
}
