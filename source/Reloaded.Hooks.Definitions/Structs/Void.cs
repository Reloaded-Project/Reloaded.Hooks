using System.Runtime.InteropServices;

namespace Reloaded.Hooks.Definitions.Structs
{
    /// <summary>
    /// Empty structure to be used in place of `Void` as return type in function pointers.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 1)]
    public struct Void { }
}