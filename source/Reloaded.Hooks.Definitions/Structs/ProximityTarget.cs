#nullable enable
using System;
using System.Diagnostics;

namespace Reloaded.Hooks.Definitions.Structs;

/// <summary>
/// Represents a target address within
/// </summary>
public class ProximityTarget
{
    private static nuint? _defaultTargetAddress;
    
    /// <summary>
    /// Target address near which to allocate to.
    /// </summary>
    public nuint TargetAddress { get; set; }
    
    /// <summary>
    /// Expected item size. [Default: 256, for safety]
    /// </summary>
    public int ItemSize { get; set; } = 256;
    
    /// <summary>
    /// Requested amount of bytes within target address to allocate the item in.
    /// </summary>
    public int RequestedProximity { get; set; } = Int32.MaxValue;

    /// <summary>
    /// Creates a proximity target used for the allocation of wrappers and other native/interop components.
    /// </summary>
    /// <param name="targetAddress">Target address near which to allocate.</param>
    /// <remarks> Note there are more properties.</remarks>
    public ProximityTarget(nuint targetAddress)
    {
        TargetAddress = targetAddress;
    }

    /// <summary>
    /// Creates a proximity target with the default value, of 0x400000 for 32-bit and
    /// 0x140000000 for 64-bit; i.e. regular module base for non-ASLR.
    /// </summary>
    public ProximityTarget()
    {
        if (_defaultTargetAddress != null)
        {
            TargetAddress = _defaultTargetAddress.Value;
            return;
        }

        var mainModule = Process.GetCurrentProcess().MainModule;
        if (mainModule != null)
        {
            // Let's hope no 128-bit process.
            _defaultTargetAddress = (nuint)(ulong)mainModule.BaseAddress;
            TargetAddress = _defaultTargetAddress.Value;
            return;
        }

        TargetAddress = (nuint)(IntPtr.Size == 4 ? 0x400000 : 0x140000000);
    }
}