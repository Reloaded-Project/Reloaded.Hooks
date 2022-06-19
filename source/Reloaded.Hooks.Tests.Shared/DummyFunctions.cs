using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Tools;
using Reloaded.Hooks.X64;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;

namespace Reloaded.Hooks.Tests.Shared
{
    public class DummyFunctions : IDisposable
    {
        /// <summary> Dummy function which returns "5". </summary>
        [Function(CallingConventions.Microsoft)]
        [Definitions.X86.Function(Definitions.X86.CallingConventions.Cdecl)]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int ReturnNumberDelegate();

        public const int Five = 5;
        public const int Six  = 6;

        public nuint ReturnFive { get; private set; }
        public nuint ReturnSix  { get; private set; }

        // Used for cleaning up function later.
        private Assembler.Assembler _assembler = new Assembler.Assembler();
        
        public unsafe DummyFunctions()
        {
            BuildReturnFive();
            BuildReturnSix();
        }

        public void Dispose()
        {
            _assembler?.Dispose();
        }

        /* Building Functions */

        public void BuildReturnFive()
        {
            string[] returnFiveFunction = new string[]
            {
                $"{_use32}",
                $"mov {_eax}, {Five}",
                $"ret"
            };

            // Note: We are deliberately creating in 32bit range because later down the road we'll be patching push + ret.
            var result = _assembler.Assemble(returnFiveFunction);
            var buffer = Utilities.FindOrCreateBufferInRange(result.Length);
            ReturnFive = buffer.Add(result);
        }

        public void BuildReturnSix()
        {
            string[] returnSixFunction = new string[]
            {
                $"{_use32}",
                $"mov {_eax}, {Six}",
                $"ret"
            };

            // Note: We are deliberately creating in 32bit range because later down the road we'll be patching push + ret.
            var result = _assembler.Assemble(returnSixFunction);
            var buffer = Utilities.FindOrCreateBufferInRange(result.Length);
            ReturnSix = buffer.Add(result);
        }
    }
}
