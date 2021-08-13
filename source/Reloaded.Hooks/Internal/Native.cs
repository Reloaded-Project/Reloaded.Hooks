using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Reloaded.Hooks.Internal
{
    internal class Native
    {
        [DllImport("kernel32.dll", SetLastError = true), SuppressUnmanagedCodeSecurity]
        internal static extern IntPtr VirtualQuery(IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, UIntPtr dwLength);

        /// <summary>
        /// <para>
        /// Contains information about a range of pages in the virtual address space of a process. The VirtualQuery and VirtualQueryEx
        /// functions use this structure.
        /// </para>
        /// </summary>
        /// <remarks>
        /// <para>
        /// To enable a debugger to debug a target that is running on a different architecture (32-bit versus 64-bit), use one of the
        /// explicit forms of this structure.
        /// </para>
        /// </remarks>
        internal struct MEMORY_BASIC_INFORMATION
        {
            /// <summary>
            /// <para>A pointer to the base address of the region of pages.</para>
            /// </summary>
            public IntPtr BaseAddress;
            /// <summary>
            /// <para>
            /// A pointer to the base address of a range of pages allocated by the VirtualAlloc function. The page pointed to by the
            /// <c>BaseAddress</c> member is contained within this allocation range.
            /// </para>
            /// </summary>
            public IntPtr AllocationBase;
            /// <summary>
            /// <para>
            /// The memory protection option when the region was initially allocated. This member can be one of the memory protection
            /// constants or 0 if the caller does not have access.
            /// </para>
            /// </summary>
            public uint AllocationProtect;
            /// <summary>
            /// <para>The size of the region beginning at the base address in which all pages have identical attributes, in bytes.</para>
            /// </summary>
            public UIntPtr RegionSize;
            /// <summary>
            /// <para>The state of the pages in the region. This member can be one of the following values.</para>
            /// <list type="table">
            /// <listheader>
            /// <term>State</term>
            /// <term>Meaning</term>
            /// </listheader>
            /// <item>
            /// <term>MEM_COMMIT 0x1000</term>
            /// <term>
            /// Indicates committed pages for which physical storage has been allocated, either in memory or in the paging file on disk.
            /// </term>
            /// </item>
            /// <item>
            /// <term>MEM_FREE 0x10000</term>
            /// <term>
            /// Indicates free pages not accessible to the calling process and available to be allocated. For free pages, the information in
            /// the AllocationBase, AllocationProtect, Protect, and Type members is undefined.
            /// </term>
            /// </item>
            /// <item>
            /// <term>MEM_RESERVE 0x2000</term>
            /// <term>
            /// Indicates reserved pages where a range of the process's virtual address space is reserved without any physical storage being
            /// allocated. For reserved pages, the information in the Protect member is undefined.
            /// </term>
            /// </item>
            /// </list>
            /// </summary>
            public uint State;
            /// <summary>
            /// <para>
            /// The access protection of the pages in the region. This member is one of the values listed for the <c>AllocationProtect</c> member.
            /// </para>
            /// </summary>
            public MEM_PROTECTION Protect;
            /// <summary>
            /// <para>The type of pages in the region. The following types are defined.</para>
            /// <list type="table">
            /// <listheader>
            /// <term>Type</term>
            /// <term>Meaning</term>
            /// </listheader>
            /// <item>
            /// <term>MEM_IMAGE 0x1000000</term>
            /// <term>Indicates that the memory pages within the region are mapped into the view of an image section.</term>
            /// </item>
            /// <item>
            /// <term>MEM_MAPPED 0x40000</term>
            /// <term>Indicates that the memory pages within the region are mapped into the view of a section.</term>
            /// </item>
            /// <item>
            /// <term>MEM_PRIVATE 0x20000</term>
            /// <term>Indicates that the memory pages within the region are private (that is, not shared by other processes).</term>
            /// </item>
            /// </list>
            /// </summary>
            public uint Type;
        }

        /// <summary>
        /// The following are the memory-protection options; you must specify one of the following values when allocating or protecting a page in memory.
        /// Protection attributes cannot be assigned to a portion of a page; they can only be assigned to a whole page.
        /// </summary>
        [Flags]
        internal enum MEM_PROTECTION : uint
        {
            /// <summary>
            /// Disables all access to the committed region of pages. An attempt to read from, write to, or execute the committed region results in an access violation.
            /// <para>This flag is not supported by the CreateFileMapping function.</para>
            /// </summary>
            PAGE_NOACCESS = 1,
            /// <summary>
            /// Enables read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation. If Data
            /// Execution Prevention is enabled, an attempt to execute code in the committed region results in an access violation.
            /// </summary>
            PAGE_READONLY = 2,
            /// <summary>
            /// Enables read-only or read/write access to the committed region of pages. If Data Execution Prevention is enabled, attempting to execute code in
            /// the committed region results in an access violation.
            /// </summary>
            PAGE_READWRITE = 4,
            /// <summary>
            /// Enables read-only or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write page
            /// results in a private copy of the page being made for the process. The private page is marked as PAGE_READWRITE, and the change is written to the
            /// new page. If Data Execution Prevention is enabled, attempting to execute code in the committed region results in an access violation.
            /// <para>This flag is not supported by the VirtualAlloc or VirtualAllocEx functions.</para>
            /// </summary>
            PAGE_WRITECOPY = 8,
            /// <summary>
            /// Enables execute access to the committed region of pages. An attempt to write to the committed region results in an access violation.
            /// <para>This flag is not supported by the CreateFileMapping function.</para>
            /// </summary>
            PAGE_EXECUTE = 16, // 0x00000010
            /// <summary>
            /// Enables execute or read-only access to the committed region of pages. An attempt to write to the committed region results in an access violation.
            /// <para>
            /// Windows Server 2003 and Windows XP: This attribute is not supported by the CreateFileMapping function until Windows XP with SP2 and Windows
            /// Server 2003 with SP1.
            /// </para>
            /// </summary>
            PAGE_EXECUTE_READ = 32, // 0x00000020
            /// <summary>
            /// Enables execute, read-only, or read/write access to the committed region of pages.
            /// <para>
            /// Windows Server 2003 and Windows XP: This attribute is not supported by the CreateFileMapping function until Windows XP with SP2 and Windows
            /// Server 2003 with SP1.
            /// </para>
            /// </summary>
            PAGE_EXECUTE_READWRITE = 64, // 0x00000040
            /// <summary>
            /// Enables execute, read-only, or copy-on-write access to a mapped view of a file mapping object. An attempt to write to a committed copy-on-write
            /// page results in a private copy of the page being made for the process. The private page is marked as PAGE_EXECUTE_READWRITE, and the change is
            /// written to the new page.
            /// <para>This flag is not supported by the VirtualAlloc or VirtualAllocEx functions.</para>
            /// <para>
            /// Windows Vista, Windows Server 2003 and Windows XP: This attribute is not supported by the CreateFileMapping function until Windows Vista with SP1
            /// and Windows Server 2008.
            /// </para>
            /// </summary>
            PAGE_EXECUTE_WRITECOPY = 128, // 0x00000080
            /// <summary>
            /// Pages in the region become guard pages. Any attempt to access a guard page causes the system to raise a STATUS_GUARD_PAGE_VIOLATION exception and
            /// turn off the guard page status. Guard pages thus act as a one-time access alarm. For more information, see Creating Guard Pages.
            /// <para>When an access attempt leads the system to turn off guard page status, the underlying page protection takes over.</para>
            /// <para>If a guard page exception occurs during a system service, the service typically returns a failure status indicator.</para>
            /// <para>This value cannot be used with PAGE_NOACCESS.</para>
            /// <para>This flag is not supported by the CreateFileMapping function.</para>
            /// </summary>
            PAGE_GUARD = 256, // 0x00000100
            /// <summary>
            /// Sets all pages to be non-cachable. Applications should not use this attribute except when explicitly required for a device. Using the interlocked
            /// functions with memory that is mapped with SEC_NOCACHE can result in an EXCEPTION_ILLEGAL_INSTRUCTION exception.
            /// <para>The PAGE_NOCACHE flag cannot be used with the PAGE_GUARD, PAGE_NOACCESS, or PAGE_WRITECOMBINE flags.</para>
            /// <para>
            /// The PAGE_NOCACHE flag can be used only when allocating private memory with the VirtualAlloc, VirtualAllocEx, or VirtualAllocExNuma functions. To
            /// enable non-cached memory access for shared memory, specify the SEC_NOCACHE flag when calling the CreateFileMapping function.
            /// </para>
            /// </summary>
            PAGE_NOCACHE = 512, // 0x00000200
            /// <summary>
            /// Sets all pages to be write-combined.
            /// <para>
            /// Applications should not use this attribute except when explicitly required for a device. Using the interlocked functions with memory that is
            /// mapped as write-combined can result in an EXCEPTION_ILLEGAL_INSTRUCTION exception.
            /// </para>
            /// <para>The PAGE_WRITECOMBINE flag cannot be specified with the PAGE_NOACCESS, PAGE_GUARD, and PAGE_NOCACHE flags.</para>
            /// <para>
            /// The PAGE_WRITECOMBINE flag can be used only when allocating private memory with the VirtualAlloc, VirtualAllocEx, or VirtualAllocExNuma
            /// functions. To enable write-combined memory access for shared memory, specify the SEC_WRITECOMBINE flag when calling the CreateFileMapping function.
            /// </para>
            /// <para>Windows Server 2003 and Windows XP: This flag is not supported until Windows Server 2003 with SP1.</para>
            /// </summary>
            PAGE_WRITECOMBINE = 1024, // 0x00000400
            /// <summary>The page contents that you supply are excluded from measurement with the EEXTEND instruction of the Intel SGX programming model.</summary>
            PAGE_ENCLAVE_UNVALIDATED = 536870912, // 0x20000000
            /// <summary>
            /// Sets all locations in the pages as invalid targets for CFG. Used along with any execute page protection like PAGE_EXECUTE, PAGE_EXECUTE_READ,
            /// PAGE_EXECUTE_READWRITE and PAGE_EXECUTE_WRITECOPY. Any indirect call to locations in those pages will fail CFG checks and the process will be
            /// terminated. The default behavior for executable pages allocated is to be marked valid call targets for CFG.
            /// <para>This flag is not supported by the VirtualProtect or CreateFileMapping functions.</para>
            /// </summary>
            PAGE_TARGETS_INVALID = 1073741824, // 0x40000000
            /// <summary>
            /// Pages in the region will not have their CFG information updated while the protection changes for VirtualProtect. For example, if the pages in the
            /// region was allocated using PAGE_TARGETS_INVALID, then the invalid information will be maintained while the page protection changes. This flag is
            /// only valid when the protection changes to an executable type like PAGE_EXECUTE, PAGE_EXECUTE_READ, PAGE_EXECUTE_READWRITE and
            /// PAGE_EXECUTE_WRITECOPY. The default behavior for VirtualProtect protection change to executable is to mark all locations as valid call targets
            /// for CFG.
            /// <para>The following are modifiers that can be used in addition to the options provided in the previous table, except as noted.</para>
            /// </summary>
            PAGE_TARGETS_NO_UPDATE = PAGE_TARGETS_INVALID, // 0x40000000
            /// <summary>The page contains a thread control structure (TCS).</summary>
            PAGE_ENCLAVE_THREAD_CONTROL = 2147483648, // 0x80000000
            /// <summary></summary>
            PAGE_REVERT_TO_FILE_MAP = PAGE_ENCLAVE_THREAD_CONTROL, // 0x80000000
        }
    }
}
