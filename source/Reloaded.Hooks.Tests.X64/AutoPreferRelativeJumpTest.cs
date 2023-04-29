using System;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Tests.Shared;
using Xunit;

namespace Reloaded.Hooks.Tests.X64;

/// <summary>
/// This class tests preferring the use of 'Relative Jumps' when an existing 0xE9 instruction is found
/// from a previous hook.
/// </summary>
/// <remarks>
///     This feature works around some bad behaviour from certain hooking libraries where they don't overwrite
///     remaining instructions with NOPs.
///
///     'Auto Prefer Relative Jump' will detect 0xE9(s) and use its own 0xE9 when possible.
/// </remarks>
public class AutoPreferRelativeJumpTest : IDisposable
{
    private NativeCalculator _nativeCalculator;
    private NativeCalculator.AddFunction _addFunction;

    private NativeCalculator _nativeCalculator2;
    private NativeCalculator.AddFunction _addFunction2;
    private bool _is64Bit;
    
    private static IHook<NativeCalculator.AddFunction> _addHook;

    public unsafe AutoPreferRelativeJumpTest()
    {
        _nativeCalculator = new NativeCalculator();
        _addFunction = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator.Add, out _);
        
        _nativeCalculator2 = new NativeCalculator();
        _addFunction2 = ReloadedHooks.Instance.CreateWrapper<NativeCalculator.AddFunction>((long)_nativeCalculator2.Add, out _);
        _is64Bit = IntPtr.Size == 8;
    }

    public void Dispose()
    {
        _nativeCalculator?.Dispose();
    }

    [Fact]
    public unsafe void TestAutoRelativeJump()
    {
        int Hookfunction(int a, int b)
        {
            return _addHook.OriginalFunction(a, b) + 1;
        }

        // Simulate Steam & Other Non-Nopping Libraries' Hook
        // by writing a jmp from function 1 to function 2, without clearing extra remaining bytes,
        // thus leaving junk after byte 5.
        var data = ReloadedHooks.Instance.Utilities.AssembleRelativeJump((nint)_nativeCalculator.Add, (nint)_nativeCalculator2.Add, _is64Bit);
        Memory.Sources.Memory.Instance.WriteRaw(_nativeCalculator.Add, data);
        
        // Simulate jump back added by external library.
        // +6/+8 is after first 3 instructions of NativeCalculator.Add
        int jumpBackOffset = _is64Bit ? 8 : 6;
        data = ReloadedHooks.Instance.Utilities.AssembleRelativeJump((nint)_nativeCalculator2.Add + jumpBackOffset, (nint)_nativeCalculator.Add + jumpBackOffset, _is64Bit);
        Memory.Sources.Memory.Instance.WriteRaw(_nativeCalculator2.Add + (nuint)jumpBackOffset, data);
        
        // Verify Function still works.
        for (int x = 0; x < 10; x++)
        {
            for (int y = 1; y < 10;)
            {
                int expected = (x + y);
                int result = _addFunction(x, y);

                Assert.Equal(expected, result);
                y += 2;
            }
        }
        
        // Now add a hook. After the 0xE9 jump, there should be some 'junk' left. This is obviously undesired.
        _addHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction>(Hookfunction, (long)_nativeCalculator.Add).Activate();

        // Verify our hook produced a 0xE9.
        Assert.Equal(0xE9, *(byte*)_nativeCalculator.Add);
        
        // Run our hook for sanity.
        for (int x = 0; x < 100; x++)
        {
            for (int y = 1; y < 100;)
            {
                int expected = (x + y) + 1;
                int result = _addFunction(x, y);

                Assert.Equal(expected, result);
                y += 2;
            }
        }
    }
}