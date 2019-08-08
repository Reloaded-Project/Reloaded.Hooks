using System;
using System.Collections.Generic;
using Reloaded.Hooks.X64;
using Reloaded.Memory.Sources;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.Tools
{
    /// <summary>
    /// Allows for easy storage of data about a virtual function table.
    /// </summary>
    public class VirtualFunctionTable
    {
        /// <summary>
        /// Stores a list of the individual table addresses for this Virtual Function Table.
        /// </summary>
        public List<TableEntry> TableEntries { get; set; }

        /// <summary>
        /// An indexer override allowing for individual Virtual Function Table
        /// entries to be easier accessed.
        /// </summary>
        /// <param name="i">The individual entry in the virtual function table.</param>
        /// <returns>The individual corresponding virtual function table entry.</returns>
        public TableEntry this[int i]
        {
            get => TableEntries[i];
            set => TableEntries[i] = value;
        }

        /// <summary>
        /// A structure type which describes the individual function pointer by its
        /// memory location and its target address.
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

        /* Factory constructor. */
        private VirtualFunctionTable() { }

        /// <summary>
        /// Initiates a virtual function table from an object address in memory.
        /// An assumption is made that the virtual function table pointer is the first parameter.
        /// </summary>
        /// <param name="objectAddress">
        ///     The memory address at which the object is stored.
        ///     The function will assume that the first entry is a pointer to the virtual function
        ///     table, as standard with C++ code.
        /// </param>
        /// <param name="numberOfMethods">
        ///     The number of methods contained in the virtual function table.
        ///     For enumerables, you may obtain this value as such: Enum.GetNames(typeof(MyEnum)).Length; where
        ///     MyEnum is the name of your enumerable.
        /// </param>
        public static VirtualFunctionTable FromObject(IntPtr objectAddress, int numberOfMethods)
        {
            VirtualFunctionTable table = new VirtualFunctionTable();
            table.TableEntries = GetObjectVTableAddresses(objectAddress, numberOfMethods);
            return table;
        }

        /// <summary>
        /// Initiates a virtual function table given the address of the first function in memory.
        /// </summary>
        /// <param name="tableAddress">
        ///     The memory address of the first entry (function pointer) of the virtual function table.
        /// </param>
        /// <param name="numberOfMethods">
        ///     The number of methods contained in the virtual function table.
        ///     For enumerables, you may obtain this value as such: Enum.GetNames(typeof(MyEnum)).Length; where
        ///     MyEnum is the name of your enumerable.
        /// </param>
        public static VirtualFunctionTable FromAddress(IntPtr tableAddress, int numberOfMethods)
        {
            VirtualFunctionTable table = new VirtualFunctionTable();
            table.TableEntries = GetAddresses(tableAddress, numberOfMethods);
            return table;
        }

        /// <summary>
        /// Generates a wrapper function for an individual virtual function table entry.
        /// </summary>
        public TFunction CreateWrapperFunction<TFunction>(int index)
        {
            if (IntPtr.Size == 4)
                return X86.Wrapper.Create<TFunction>((long)TableEntries[index].FunctionPointer, out var wrapperAddress);
            if (IntPtr.Size == 8)
                return Wrapper.Create<TFunction>((long)TableEntries[index].FunctionPointer, out var wrapperAddress);

            throw new Exception("Machine does not appear to be of a 32 or 64bit architecture.");
        }

        /// <summary>
        /// Hooks an individual virtual function table entry in a virtual function table.
        /// </summary>
        public IHook<TFunction> CreateFunctionHook<TFunction>(int index, TFunction delegateType)
        {
            return new Hook<TFunction>(delegateType, (long)TableEntries[index].FunctionPointer);
        }

        /*
            ---------------
            Factory Methods
            ---------------
        */

        private static List<TableEntry> GetObjectVTableAddresses(IntPtr objectAddress, int numberOfMethods)
        {
            CurrentProcess.SafeRead(objectAddress, out IntPtr virtualFunctionTableAddress);
            return GetAddresses(virtualFunctionTableAddress, numberOfMethods);
        }

        private static List<TableEntry> GetAddresses(IntPtr tablePointer, int numberOfMethods)
        {
            // Stores the addresses of the virtual function table.
            List<TableEntry> tablePointers = new List<TableEntry>();

            // Append the table pointers onto the tablePointers list.
            // Using the size of the IntPtr allows for both x64 and x86 support.
            for (int i = 0; i < numberOfMethods; i++)
            {
                IntPtr targetAddress = tablePointer + (IntPtr.Size * i);

                CurrentProcess.SafeRead(targetAddress, out IntPtr functionPtr);
                tablePointers.Add(new TableEntry {
                    EntryAddress = targetAddress,
                    FunctionPointer = functionPtr
                });
            }

            return tablePointers;
        }
    }
}
