using System;

namespace Reloaded.Hooks.Definitions
{
    /// <summary>
    /// A structure type which an individual virtual function table entry.
    /// </summary>
    public struct TableEntry
    {
        /// <summary>
        /// The address in process memory where the VTable entry has been found.
        /// </summary>
        public IntPtr EntryAddress;

        /// <summary>
        /// The value of the individual entry in process memory for the VTable entry pointing to a function.
        /// </summary>
        public IntPtr FunctionPointer;
    }
}
