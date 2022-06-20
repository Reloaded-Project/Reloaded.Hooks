using static Reloaded.Memory.Buffers.Internal.Kernel32.Kernel32;
using System.Diagnostics;
using System;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Buffers;

namespace Reloaded.Hooks.Tests.Shared;

public interface IMemoryAllocator
{
    nuint Allocate(int size);

    public nuint Write(byte[] data)
    {
        var ptr = Allocate(data.Length);
        Memory.Sources.Memory.CurrentProcess.WriteRaw(ptr, data);
        return ptr;
    }
}

public class ReloadedMemoryAllocator : IMemoryAllocator
{
    public nuint Allocate(int size) => Memory.Sources.Memory.CurrentProcess.Allocate(size);
}

public class LowMemoryAllocator : IMemoryAllocator
{
    private nuint _minAddress;
    private nuint _maxAddress;
    private MemoryBufferHelper _helper = new(Process.GetCurrentProcess());

    public LowMemoryAllocator(nuint size = 0x10000000) // 256MB
    {
        _maxAddress = size + 1;
        _minAddress = 1;
    }

    public nuint Allocate(int size)
    {
        var buf = Utilities.FindOrCreateBufferInRange(size, _minAddress, _maxAddress);
        return buf.Add(new byte[size]);
    }
}

public class HighMemoryAllocator : IMemoryAllocator
{
    private nuint _minAddress;
    private nuint _maxAddress;
    private MemoryBufferHelper _helper = new(Process.GetCurrentProcess());

    public HighMemoryAllocator(nuint size = 0x1F00000)
    {
        _maxAddress = MemoryAllocatorHelpers.GetMaxAddress(true);
        _minAddress = (_maxAddress - size);
    }

    public nuint Allocate(int size)
    {
        var buf = Utilities.FindOrCreateBufferInRange(size, _minAddress, _maxAddress);
        return buf.Add(new byte[size]);
    }
}

public class MemoryAllocatorHelpers
{
    /// <summary>
    /// Returns the max addressable address of the process sitting behind the <see cref="MemoryBufferHelper"/>.
    /// </summary>
    public static UIntPtr GetMaxAddress(bool largeAddressAware = false)
    {
        // Is this Windows on Windows 64? (x86 app running on x64 Windows)
        IsWow64Process(Process.GetCurrentProcess().Handle, out bool isWow64);
        GetSystemInfo(out SYSTEM_INFO systemInfo);
        long maxAddress = 0x7FFFFFFF;

        // Check for large address aware
        if (largeAddressAware && IntPtr.Size == 4 && (uint)systemInfo.lpMaximumApplicationAddress > maxAddress)
            maxAddress = (uint)systemInfo.lpMaximumApplicationAddress;

        // Check if 64bit.
        if (systemInfo.wProcessorArchitecture == ProcessorArchitecture.PROCESSOR_ARCHITECTURE_AMD64 && !isWow64)
            maxAddress = (long)systemInfo.lpMaximumApplicationAddress;

        return (UIntPtr)maxAddress;
    }
}