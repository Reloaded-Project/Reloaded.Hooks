using System;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Benchmarks
{
    public class NativeCalculator64 : NativeCalculator, IDisposable
    {
        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        private Memory.Sources.Memory _memory  = new Memory.Sources.Memory();

        public NativeCalculator64()
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
                $"use64",
                $"lea eax, [rcx+rdx]",
                $"ret",
            };

            var result = _assembler.Assemble(addFunction);
            Add = _memory.Allocate(result.Length);
            _memory.WriteRaw(Add, result);
        }

    }
}
