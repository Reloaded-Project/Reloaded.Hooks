using System;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.X64;
using Reloaded.Hooks.Tests.Shared;
using Reloaded.Hooks.Tools;
using Xunit;
using static Reloaded.Hooks.Definitions.X64.FunctionAttribute;
using static Reloaded.Hooks.Tests.Shared.Macros.Macros;
using static Reloaded.Memory.Sources.Memory;
using CallingConventions = Reloaded.Hooks.Definitions.X86.CallingConventions;

namespace Reloaded.Hooks.Tests.X64;

/// <summary>
/// Tests if jumps that go beyond possible limit can be replaced with long variants.
/// </summary>
public class LongJumpTest
{
    private IMemoryAllocator _lowAlloc = new LowMemoryAllocator();
    private IMemoryAllocator _highMemoryAllocator = new HighMemoryAllocator();

    [Function(new Register[0] { }, Register.rax, false)]
    [Definitions.X86.Function(CallingConventions.Cdecl)]
    public delegate int GetValueFunction();

    [Fact]
    public void LongRelativeJump()
    {
        using var assembler = new Assembler.Assembler();
        const int expectedResult = 42069;
        
        string[] customFunction = new string[]
        {
            $"{_use32}",
            $"mov {_eax}, {expectedResult}",
            $"ret"
        };
        
        // Make target and source.
        var target = _highMemoryAllocator.Write(assembler.Assemble(customFunction));
        var src    = _lowAlloc.Allocate(100); // for our jump instruction

        // Assert original works.
        var tgtMethod = ReloadedHooks.Instance.CreateFunction<GetValueFunction>(target.ToSigned());
        Assert.Equal(expectedResult, tgtMethod.GetWrapper()());

        CurrentProcess.WriteRaw(src, Utilities.AssembleRelativeJump(src, target, Environment.Is64BitProcess));

        // Call the code.
        var srcMethod = ReloadedHooks.Instance.CreateFunction<GetValueFunction>(src.ToSigned());
        Assert.Equal(expectedResult, srcMethod.GetWrapper()());
    }
}