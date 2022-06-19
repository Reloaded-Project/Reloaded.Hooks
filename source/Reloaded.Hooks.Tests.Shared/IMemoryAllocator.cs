using static Reloaded.Memory.Buffers.Internal.Kernel32.Kernel32;
using System.Diagnostics;
using System;

namespace Reloaded.Hooks.Tests.Shared;

public interface IMemoryAllocator
{
    nuint Allocate(int size);
}

public class ReloadedMemoryAllocator : IMemoryAllocator
{
    public nuint Allocate(int size) => Memory.Sources.Memory.CurrentProcess.Allocate(size);
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