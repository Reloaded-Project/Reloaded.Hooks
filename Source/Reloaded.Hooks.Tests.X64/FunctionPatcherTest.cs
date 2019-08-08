using System;
using System.Linq;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Internal.Testing;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Reloaded.Hooks.X64;
using SharpDisasm;
using Xunit;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.Tests.X64
{
    public class FunctionPatcherTest : IDisposable
    {
        private const ArchitectureMode ArchitectureMode = SharpDisasm.ArchitectureMode.x86_64;

        private DummyFunctions _dummyFunctions;

        private DummyFunctions.ReturnNumberDelegate _returnFive;
        private DummyFunctions.ReturnNumberDelegate _returnSix;

        private Assembler.Assembler _assembler;

        // Pointers to stub functions redirecting to ReturnFive.
        // Test calling them; then test what FunctionPatcher thinks about them.
        private IntPtr _relativeJmpPtr;
        private IntPtr _pushReturnPtr;
        private IntPtr _ripRelativePtr; // Remove for X86

        private int _relativeJmpLength;
        private int _pushReturnLength;
        private int _ripRelativeLength;

        public FunctionPatcherTest()
        {
            _dummyFunctions = new DummyFunctions();
            _returnFive = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long) _dummyFunctions.ReturnFive);
            _returnSix  = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)_dummyFunctions.ReturnSix);
            _assembler = new Assembler.Assembler();

            BuildRelativeJmp();
            BuildPushReturn();
            BuildRIPRelativeJmp();
        }

        public void Dispose()
        {
            _dummyFunctions?.Dispose();
        }

        /* Build Jump methods for Patching */

        private void BuildRelativeJmp()
        {
            var minMax = Utilities.GetRelativeJumpMinMax((long) _dummyFunctions.ReturnFive);
            var buffer = Utilities.FindOrCreateBufferInRange(32, minMax.min, minMax.max);

            long jmpSource = (long)buffer.Properties.WritePointer;
            long jmpTarget = (long) _dummyFunctions.ReturnFive;
            
            var relativeJmp = new string[]
            {
                $"{_use32}",
                $"jmp {jmpTarget - jmpSource}"
            };

            var asm = _assembler.Assemble(relativeJmp);
            _relativeJmpLength = asm.Length;
            _relativeJmpPtr = buffer.Add(asm);
        }

        private void BuildPushReturn()
        {
            var buffer = Utilities.FindOrCreateBufferInRange(32);
            long jmpTarget = (long)_dummyFunctions.ReturnFive;

            var pushReturn = new string[]
            {
                $"{_use32}",
                $"push {jmpTarget}",
                "ret"
            };

            var asm = _assembler.Assemble(pushReturn);
            _pushReturnLength = asm.Length;
            _pushReturnPtr = buffer.Add(asm);
        }

        private void BuildRIPRelativeJmp()
        {
            var buffer = Utilities.FindOrCreateBufferInRange(32);
            long jmpTarget = (long)_dummyFunctions.ReturnFive;

            var ripRelativeJmp = new string[]
            {
                $"{_use32}",
                $"jmp qword [rip + 0]" // FASM offsets from end of instruction.
            };

            var asm = _assembler.Assemble(ripRelativeJmp);
            _ripRelativeLength = asm.Length;
            _ripRelativePtr = buffer.Add(asm);
            buffer.Add(ref jmpTarget, false, 1);

            // TODO: Hack! SharpDisasm does not correctly disasm multiple `jmp [rip + 0]` opcodes in sequence because of mixing code + data (pointer).
            // Technically we shouldn't mix code with data, but I did it here to make life easier for testing.
            // We can also "fix" this by running this function in a different order but preferably should one
            // day see if can patch disassembler.
            byte[] int3Bytes = { 0xCC, 0xCC, 0xCC, 0xCC };
            buffer.Add(int3Bytes);
        }

        /* Test return address patching by patching returns originally pointing to ReturnSix to ReturnFive */

        [Fact]
        public void CallPatchedRIPRelativeReturnJump()
        {
            // Build RIP Relative Jump to ReturnSix
            var buffer     = Utilities.FindOrCreateBufferInRange(100);
            long jmpTarget = (long)_dummyFunctions.ReturnSix;

            var ripRelativeJmp = new string[]
            {
                $"{_use32}",
                $"jmp qword [rip + 0]" // FASM offsets from end of instruction.
            };

            var asm = _assembler.Assemble(ripRelativeJmp);
            var relativePtr = buffer.Add(asm);
            buffer.Add(ref jmpTarget, false, 1);

            // TODO: Hack! SharpDisasm does not correctly disasm multiple `jmp [rip + 0]` opcodes in sequence because of mixing code + data (pointer).
            // See other instance of this for notes.
            byte[] int3Bytes = { 0xCC, 0xCC, 0xCC, 0xCC };
            buffer.Add(int3Bytes);

            // Create a wrapper and call to confirm jump works.
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)relativePtr);
            Assert.Equal(DummyFunctions.Six, wrapper());

            // Now try to retarget jump to ReturnFive
            var patcher = new FunctionPatcher(ArchitectureMode);
            long searchTarget = jmpTarget;
            FunctionPatcherTesting.GetSearchRange(patcher, ref searchTarget, out long searchLength);

            var patches = FunctionPatcherTesting.PatchJumpTargets (patcher, 
                new AddressRange(searchTarget, searchTarget + searchLength),
                new AddressRange(jmpTarget, jmpTarget), 
                (long)_dummyFunctions.ReturnFive);

            Assert.True(patches.Count > 0);

            foreach (var patch in patches)
            { patch.Apply(); }

            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        [Fact]
        public void CallPatchedPushReturnReturnJump()
        {
            // Build RIP Relative Jump to ReturnSix
            var buffer = Utilities.FindOrCreateBufferInRange(100);
            long jmpTarget = (long)_dummyFunctions.ReturnSix;

            var pushReturnJmp = new string[]
            {
                $"{_use32}",
                $"push {jmpTarget}",
                "ret"
            };

            var asm = _assembler.Assemble(pushReturnJmp);
            var pushReturnPtr = buffer.Add(asm);
            
            // Create a wrapper and call to confirm jump works.
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)pushReturnPtr);
            Assert.Equal(DummyFunctions.Six, wrapper());

            // Now try to retarget jump to ReturnFive
            var patcher = new FunctionPatcher(ArchitectureMode);
            long searchTarget = jmpTarget;
            FunctionPatcherTesting.GetSearchRange(patcher, ref searchTarget, out long searchLength);

            var patches = FunctionPatcherTesting.PatchJumpTargets(patcher,
                new AddressRange(searchTarget, searchTarget + searchLength),
                new AddressRange(jmpTarget, jmpTarget),
                (long)_dummyFunctions.ReturnFive);

            Assert.True(patches.Count > 0);

            foreach (var patch in patches)
            { patch.Apply(); }

            Assert.Equal(DummyFunctions.Five, wrapper());
        }


        [Fact]
        public void CallPatchedRelativeReturnJump()
        {
            // Build RIP Relative Jump to ReturnSix
            var buffer = Utilities.FindOrCreateBufferInRange(100);
            long jmpTarget = (long)_dummyFunctions.ReturnSix;

            var relativeJmp = new string[]
            {
                $"{_use32}",
                $"jmp {(long)jmpTarget - (long)buffer.Properties.WritePointer}", // FASM relative offsets are relative to start of instruction.
            };

            var asm = _assembler.Assemble(relativeJmp);
            var relativePtr = buffer.Add(asm, 1);

            // Create a wrapper and call to confirm jump works.
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)relativePtr);
            Assert.Equal(DummyFunctions.Six, wrapper());

            // Now try to retarget jump to ReturnFive
            var patcher = new FunctionPatcher(ArchitectureMode);
            long searchTarget = jmpTarget;
            FunctionPatcherTesting.GetSearchRange(patcher, ref searchTarget, out long searchLength);

            var patches = FunctionPatcherTesting.PatchJumpTargets(patcher,
                new AddressRange(searchTarget, searchTarget + searchLength),
                new AddressRange(jmpTarget, jmpTarget),
                (long)_dummyFunctions.ReturnFive);

            Assert.True(patches.Count > 0);

            foreach (var patch in patches)
            { patch.Apply(); }

            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        /* Test Calling via Stubs to ReturnFive */

        [Fact]
        public void CallReturnFive()
        {
            Assert.Equal(DummyFunctions.Five, _returnFive());
        }

        [Fact]
        public void CallReturnSix()
        {
            Assert.Equal(DummyFunctions.Six, _returnSix());
        }

        [Fact]
        public void CallRelativeJmp()
        {
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long) _relativeJmpPtr);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        [Fact]
        public void CallPushReturn()
        {
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)_pushReturnPtr);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        [Fact]
        public void CallRIPRelative()
        {
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)_ripRelativePtr);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }


        /* Test calling rewritten position independent methods using absolute jumps. */

        [Fact]
        public void CallRewrittenRelativeJmp()
        {
            var functionPatcher = new FunctionPatcher(ArchitectureMode);
            CurrentProcess.ReadRaw(_relativeJmpPtr, out byte[] originalBytes, _relativeJmpLength);
            var patch = functionPatcher.Patch(originalBytes.ToList(), _relativeJmpPtr);

            var buffer = Utilities.FindOrCreateBufferInRange(patch.NewFunction.Count);
            var newFunctionAddress = buffer.Add(patch.NewFunction.ToArray());
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)newFunctionAddress);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        [Fact]
        public void CallRewrittenPushReturn()
        {
            var functionPatcher = new FunctionPatcher(ArchitectureMode);
            CurrentProcess.ReadRaw(_pushReturnPtr, out byte[] originalBytes, _pushReturnLength);
            var patch = functionPatcher.Patch(originalBytes.ToList(), _pushReturnPtr);

            var buffer = Utilities.FindOrCreateBufferInRange(patch.NewFunction.Count);
            var newFunctionAddress = buffer.Add(patch.NewFunction.ToArray());
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)newFunctionAddress);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }

        [Fact]
        public void CallRewrittenRIPJump()
        {
            var functionPatcher = new FunctionPatcher(ArchitectureMode);
            CurrentProcess.ReadRaw(_ripRelativePtr, out byte[] originalBytes, _ripRelativeLength);
            var patch = functionPatcher.Patch(originalBytes.ToList(), _ripRelativePtr);

            var buffer = Utilities.FindOrCreateBufferInRange(patch.NewFunction.Count);
            var newFunctionAddress = buffer.Add(patch.NewFunction.ToArray());
            var wrapper = Wrapper.Create<DummyFunctions.ReturnNumberDelegate>((long)newFunctionAddress);
            Assert.Equal(DummyFunctions.Five, wrapper());
        }
    }
}
