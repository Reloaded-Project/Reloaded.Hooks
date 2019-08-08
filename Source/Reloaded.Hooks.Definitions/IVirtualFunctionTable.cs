using System.Collections.Generic;

namespace Reloaded.Hooks.Definitions
{
    public interface IVirtualFunctionTable
    {
        /// <summary>
        /// Stores a list of the individual table addresses for this Virtual Function Table.
        /// </summary>
        List<TableEntry> TableEntries { get; set; }

        /// <summary>
        /// An indexer override allowing for individual Virtual Function Table
        /// entries to be easier accessed.
        /// </summary>
        /// <param name="i">The individual entry in the virtual function table.</param>
        /// <returns>The individual corresponding virtual function table entry.</returns>
        TableEntry this[int i] { get; set; }

        /// <summary>
        /// Generates a wrapper function for an individual virtual function table entry.
        /// </summary>
        TFunction CreateWrapperFunction<TFunction>(int index);

        /// <summary>
        /// Hooks an individual virtual function table entry in a virtual function table.
        /// </summary>
        IHook<TFunction> CreateFunctionHook<TFunction>(int index, TFunction delegateType);
    }
}