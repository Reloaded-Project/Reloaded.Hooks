using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using static Reloaded.Memory.Sources.Memory;
using Reloaded.Memory.Sources;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Tools
{
    /// <summary>
    /// Hooks an object's virtual function table from an object address in memory.
    /// Only this object is hooked, other objects are unaffected.
    /// 
    /// An assumption is made that the virtual function table pointer is the first parameter.
    /// This hooks the virtual function table pointer by copying the object's virtual function table
    /// and changing the objects virtual function table pointer to the address of the copy.
    /// 
    /// After calling this, individual functions must be hooked by calling CreateFunctionHook.
    /// </summary>
    public class HookedObjectVirtualFunctionTable : IVirtualFunctionTable
    {
        internal class VTableEntryHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction> : IHook<TFunction>
        {
            private readonly HookedObjectVirtualFunctionTable _vTableHook;
            private readonly int _index;
            private readonly bool _is64Bit;

            /// <summary>
            /// Hooks an individual vtable function pointer from a already hooked vtable pointer.
            /// </summary>
            /// <param name="vTableHook">The already hooked virtual function table</param>
            /// <param name="originalVirtualFunctionTableAddress">The address of the original virtual function pointer
            /// This will be read to store the original function pointer</param>
            /// <param name="index">The index of the virtual function pointer in the virtual function table</param>
            /// <param name="function">The hook function</param>
            internal unsafe VTableEntryHook(HookedObjectVirtualFunctionTable vTableHook, nuint originalVirtualFunctionTableAddress, int index,
                TFunction function)
            {
                CurrentProcess.SafeRead(originalVirtualFunctionTableAddress + (nuint)(index * sizeof(nuint)), out nuint originalFunctionAddress);

                _vTableHook = vTableHook;
                _index = index;
                _is64Bit = sizeof(IntPtr) == 8;
                ReverseWrapper = CreateReverseWrapper(function);
                OriginalFunction = CreateWrapper(originalFunctionAddress, out nuint originalFunctionWrapperAddress);
                OriginalFunctionAddress = originalFunctionAddress.ToSigned();
                OriginalFunctionWrapperAddress = originalFunctionWrapperAddress.ToSigned();
                IsHookActivated = false;
            }

            /// <inheritdoc />
            public TFunction OriginalFunction { get; }

            /// <inheritdoc />
            public IReverseWrapper<TFunction> ReverseWrapper { get; }

            /// <inheritdoc />
            public bool IsHookEnabled { get; private set; }

            /// <inheritdoc />
            public bool IsHookActivated { get; private set; }

            /// <inheritdoc />
            public IntPtr OriginalFunctionAddress { get; }

            /// <inheritdoc />
            public IntPtr OriginalFunctionWrapperAddress { get; }

            /// <inheritdoc />
            protected IReverseWrapper<TFunction> CreateReverseWrapper(TFunction function)
            {
                if (_is64Bit)
                    return new X64.ReverseWrapper<TFunction>(function);

                return new X86.ReverseWrapper<TFunction>(function);
            }

            /// <inheritdoc />
            protected TFunction CreateWrapper(nuint functionAddress, out nuint wrapperAddress)
            {
                if (_is64Bit)
                    return X64.Wrapper.Create<TFunction>(functionAddress, out wrapperAddress);

                return X86.Wrapper.Create<TFunction>(functionAddress, out wrapperAddress);
            }

            /// <inheritdoc />
            IHook IHook.Activate() => Activate();

            /// <inheritdoc />
            public IHook<TFunction> Activate()
            {
                _vTableHook.AddHook(this);
                Enable();
                IsHookActivated = true;
                return this;
            }

            /// <inheritdoc />
            public void Disable()
            {
                _vTableHook.SetHookedVTableEntry(_index, OriginalFunctionAddress.ToUnsigned());
                IsHookEnabled = false;
            }

            /// <inheritdoc />
            public void Enable()
            {
                _vTableHook.SetHookedVTableEntry(_index, ReverseWrapper.WrapperPointer.ToUnsigned());
                IsHookEnabled = true;
            }
        }

        /// <inheritdoc />
        public List<TableEntry> TableEntries { get; set; }

        /// <summary>
        /// The address of the hooked object
        /// </summary>
        public IntPtr ObjectAddress { get; private set; }

        /// <inheritdoc />
        public TableEntry this[int i]
        {
            get => TableEntries[i];
            set => TableEntries[i] = value;
        }

        private readonly nuint _originalVirtualFunctionTableAddress;
        private readonly nuint[] _newVTableForObject;
        private readonly GCHandle _pinnedNewVTableForObject;
        private readonly List<IHook> _hooks;

        /// <summary>
        /// Hooks an object's virtual function table from an object address in memory.
        /// Only this object is hooked, other objects are unaffected.
        /// An assumption is made that the virtual function table pointer is the first parameter.
        /// This hooks the virtual function table pointer by copying the object's virtual function table
        /// and changing the objects virtual function table pointer to the address of the copy.
        /// After calling this, individual functions must be hooked by calling CreateFunctionHook.
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
        ///     Make sure this number is at least as big as your target vtable, 
        ///     as we need to copy -all- of the vtable function pointers.
        /// </param>
        public static HookedObjectVirtualFunctionTable FromObject(nuint objectAddress, int numberOfMethods)
        {
            CurrentProcess.SafeRead(objectAddress, out nuint virtualFunctionTableAddress);
            var table = VirtualFunctionTableHelpers.GetAddresses(virtualFunctionTableAddress, numberOfMethods);
            return new HookedObjectVirtualFunctionTable(objectAddress, virtualFunctionTableAddress, table);
        }

        /// <inheritdoc />
        public IHook<TFunction> CreateFunctionHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(int index, TFunction delegateType)
        {
            return new VTableEntryHook<TFunction>(this, _originalVirtualFunctionTableAddress, index, delegateType);
        }

        /// <inheritdoc />
        public unsafe TFunction CreateWrapperFunction<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(int index)
        {
            if (sizeof(IntPtr) == 4)
                return X86.Wrapper.Create<TFunction>(TableEntries[index].FunctionPointer.ToUnsigned(), out var wrapperAddress);
            if (sizeof(IntPtr) == 8)
                return X64.Wrapper.Create<TFunction>(TableEntries[index].FunctionPointer.ToUnsigned(), out var wrapperAddress);

            throw new Exception("Machine does not appear to be of a 32 or 64bit architecture.");
        }

        private HookedObjectVirtualFunctionTable(nuint objectAddress, nuint virtualFunctionTableAddress, List<TableEntry> table)
        {
            ObjectAddress = objectAddress.ToSigned();
            TableEntries = table;

            _originalVirtualFunctionTableAddress = virtualFunctionTableAddress;
            _newVTableForObject = new nuint[table.Count];
            _hooks = new List<IHook>();
            for (int i = 0; i < table.Count; i++)
                _newVTableForObject[i] = table[i].FunctionPointer.ToUnsigned();

            _pinnedNewVTableForObject = GCHandle.Alloc(_newVTableForObject, GCHandleType.Pinned);
            CurrentProcess.SafeWrite(objectAddress, _pinnedNewVTableForObject.AddrOfPinnedObject());
        }

        private void SetHookedVTableEntry(int index, nuint newEntry)
        {
            _newVTableForObject[index] = newEntry;
        }

        /// <summary>
        /// Adds the individual vtable hook to a list so it can not be garbage collected, while the main object is still active
        /// </summary>
        /// <param name="hook">The hook to add</param>
        private void AddHook(IHook hook)
        {
            _hooks.Add(hook);
        }
    }
}
