using System;
using System.Collections.Generic;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.X64;
using Reloaded.Memory.Sources;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.Tools
{
    /// <summary>
    /// Allows for easy storage of data about a virtual function table.
    /// </summary>
    public class VirtualFunctionTable : IVirtualFunctionTable
    {
        /// <inheritdoc />
        public List<TableEntry> TableEntries { get; set; }

        /// <inheritdoc />
        public TableEntry this[int i]
        {
            get => TableEntries[i];
            set => TableEntries[i] = value;
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
            table.TableEntries = VirtualFunctionTableHelpers.GetAddresses(tableAddress, numberOfMethods);
            return table;
        }

        /// <inheritdoc />
        public unsafe TFunction CreateWrapperFunction<TFunction>(int index)
        {
            if (sizeof(IntPtr) == 4)
                return X86.Wrapper.Create<TFunction>((long)TableEntries[index].FunctionPointer, out var wrapperAddress);
            if (sizeof(IntPtr) == 8)
                return Wrapper.Create<TFunction>((long)TableEntries[index].FunctionPointer, out var wrapperAddress);

            throw new Exception("Machine does not appear to be of a 32 or 64bit architecture.");
        }

        /// <inheritdoc />
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
            return VirtualFunctionTableHelpers.GetAddresses(virtualFunctionTableAddress, numberOfMethods);
        }
    }
}

/// <summary>
/// Helper classes for dealing with virtual function tables.
/// </summary>
public static class VirtualFunctionTableHelpers
{
    /// <summary>
    /// Gets all addresses belonging to a virtual function table.
    /// </summary>
    /// <param name="tablePointer">Pointer to the virtual table itself.</param>
    /// <param name="numberOfMethods">The number of methods stored in the virtual table.</param>
    public static List<TableEntry> GetAddresses(IntPtr tablePointer, int numberOfMethods)
    {
        // Stores the addresses of the virtual function table.
        var tablePointers = new List<TableEntry>();

        // Append the table pointers onto the tablePointers list.
        // Using the size of the IntPtr allows for both x64 and x86 support.
        for (int i = 0; i < numberOfMethods; i++)
        {
            var targetAddress = tablePointer + (IntPtr.Size * i);

            CurrentProcess.SafeRead(targetAddress, out IntPtr functionPtr);
            tablePointers.Add(new TableEntry
            {
                EntryAddress = targetAddress,
                FunctionPointer = functionPtr
            });
        }

        return tablePointers;
    }
}