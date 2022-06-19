using System.Runtime.CompilerServices;

namespace Reloaded.Hooks.Definitions.Helpers;

/// <summary>
/// Useful extensions for dealing with memory addresses.
/// </summary>
public static class AddressExtensions
{
    /// <summary>
    /// Converts a signed long to an unsigned long.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static ulong ToUnsigned(this long value) => Unsafe.As<long, ulong>(ref value);

    /// <summary>
    /// Converts a signed nint/IntPtr to a native unsigned value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static nuint ToUnsigned(this nint value) => Unsafe.As<nint, nuint>(ref value);

    /// <summary>
    /// Converts an unsigned nuint/UIntPtr to a native signed value.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static nint ToSigned(this nuint value) => Unsafe.As<nuint, nint>(ref value);
}