using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions.Internal;

namespace Reloaded.Hooks.Definitions.X86
{
    /// <summary>
    /// Stores function information for custom functions.
    /// See <see cref="CallingConventions" /> for information common calling convention settings.
    /// </summary>
    public class FunctionAttribute : Attribute, IFunctionAttribute
    {
        public static Register[] DefaultSavedRegisters { get; } = new[] { Register.ebx, Register.esi, Register.edi };
        public static FunctionAttribute Cdecl = new FunctionAttribute(CallingConventions.Cdecl);
        public static FunctionAttribute StdCall = new FunctionAttribute(CallingConventions.Stdcall);
        public static FunctionAttribute Fastcall = new FunctionAttribute(CallingConventions.Fastcall);
        public static FunctionAttribute GccThiscall = new FunctionAttribute(CallingConventions.GCCThiscall);
        public static FunctionAttribute MicrosoftThiscall = new FunctionAttribute(CallingConventions.MicrosoftThiscall);

        /// <inheritdoc />
        public Register[] SourceRegisters { get; }

        /// <inheritdoc />
        public Register ReturnRegister { get; }

        /// <inheritdoc />
        public StackCleanup Cleanup { get; }

        /// <inheritdoc />
        public int ReservedStackSpace { get; }

        /// <inheritdoc />
        public Register[] CalleeSavedRegisters { get; } = DefaultSavedRegisters;

        [SuppressMessage("ReSharper", "InconsistentNaming")]
        public enum Register
        {
            eax,
            ebx,
            ecx,
            edx,
            esi,
            edi,
            ebp,
            esp
        }
        
        public enum StackCleanup
        {
            None,
            Caller,
            Callee
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegisters">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="stackCleanup">Defines the stack cleanup rule for the function. See <see cref="StackCleanup"/> for more details.</param>
        /// <param name="reservedStackSpace">Allocates an extra amount of uninitialized (not zero-written) stack space for the function to use when calling. Required by some compiler optimized functions.</param>
        public FunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, int reservedStackSpace = 0)
        {
            SourceRegisters = sourceRegisters;
            ReturnRegister = returnRegister;
            Cleanup = stackCleanup;
            ReservedStackSpace = reservedStackSpace;
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegisters">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="stackCleanup">Defines the stack cleanup rule for the function. See <see cref="StackCleanup"/> for more details.</param>
        /// <param name="calleeSavedRegisters">A list of registers that should be preserved by this function.</param>
        /// <param name="reservedStackSpace">Allocates an extra amount of uninitialized (not zero-written) stack space for the function to use when calling. Required by some compiler optimized functions.</param>
        public FunctionAttribute(Register[] sourceRegisters, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace = 0)
        {
            SourceRegisters = sourceRegisters;
            ReturnRegister = returnRegister;
            Cleanup = stackCleanup;
            ReservedStackSpace = reservedStackSpace;
            CalleeSavedRegisters = calleeSavedRegisters;
        }

        /// <summary>
        /// Initializes a ReloadedFunction with its default parameters supplied in the constructor.
        /// </summary>
        /// <param name="sourceRegister">Registers in left to right parameter order passed to the custom function.</param>
        /// <param name="returnRegister">The register that the function returns its value in.</param>
        /// <param name="stackCleanup">Defines the stack cleanup rule for the function. See <see cref="StackCleanup"/> for more details.</param>
        /// <param name="calleeSavedRegisters">A list of registers that should be preserved by this function.</param>
        /// <param name="reservedStackSpace">Allocates an extra amount of uninitialized (not zero-written) stack space for the function to use when calling. Required by some compiler optimized functions.</param>
        public FunctionAttribute(Register sourceRegister, Register returnRegister, StackCleanup stackCleanup, Register[] calleeSavedRegisters, int reservedStackSpace = 0)
        {
            SourceRegisters = new[] { sourceRegister };
            ReturnRegister = returnRegister;
            Cleanup = stackCleanup;
            ReservedStackSpace = reservedStackSpace;
            CalleeSavedRegisters = calleeSavedRegisters;
        }

        /// <summary>
        /// Initializes the ReloadedFunction using a preset calling convention.
        /// </summary>
        /// <param name="callingConvention">The calling convention preset to use for instantiating the ReloadedFunction.</param>
        public FunctionAttribute(CallingConventions callingConvention)
        {
            switch (callingConvention)
            {
                case CallingConventions.Cdecl:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    CalleeSavedRegisters = DefaultSavedRegisters;
                    break;

                case CallingConventions.Stdcall:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Callee;
                    CalleeSavedRegisters = DefaultSavedRegisters;
                    break;

                case CallingConventions.Fastcall:
                    SourceRegisters = new []{ Register.ecx, Register.edx };
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    CalleeSavedRegisters = DefaultSavedRegisters;
                    break;

                case CallingConventions.MicrosoftThiscall:
                    SourceRegisters = new[] { Register.ecx };
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Callee;
                    CalleeSavedRegisters = DefaultSavedRegisters;
                    break;

                case CallingConventions.GCCThiscall:
                    SourceRegisters = new Register[0];
                    ReturnRegister = Register.eax;
                    Cleanup = StackCleanup.Caller;
                    CalleeSavedRegisters = DefaultSavedRegisters;
                    break;

                default:
                    throw new ArgumentException($"There is no preset for the specified calling convention {callingConvention.GetType().Name}");
            }
        }


        /// <inheritdoc />
        public bool IsEquivalent(UnmanagedFunctionPointerAttribute attribute)
        {
            // Default convention is StdCall on Windows, Cdecl elsewhere.
            if (attribute == null)
                return this.Equals(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StdCall : Cdecl);

            return attribute.CallingConvention switch
            {
                CallingConvention.Cdecl => this.Equals(Cdecl),
                CallingConvention.StdCall => this.Equals(StdCall),
                CallingConvention.ThisCall => this.Equals(MicrosoftThiscall),

                // Not supported by runtime, is new or depends on platform
                _ => false,
            };
        }

        /// <inheritdoc />
        public IFunctionAttribute GetEquivalent(UnmanagedFunctionPointerAttribute attribute)
        {
            if (attribute == null)
                return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StdCall : Cdecl;

            return attribute.CallingConvention switch
            {
                CallingConvention.Cdecl => Cdecl,
                CallingConvention.StdCall => StdCall,
                CallingConvention.ThisCall => MicrosoftThiscall,
                CallingConvention.FastCall => Fastcall,
                CallingConvention.Winapi => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StdCall : Cdecl,

                // Not supported by runtime or is new
                _ => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? StdCall : Cdecl
            };
        }

        /// <summary>
        /// Retrieves a ReloadedFunction attribute from a supplied delegate type.
        /// </summary>
        public static IFunctionAttribute GetAttribute<TFunction>()
        {
            if (Misc.TryGetAttribute<TFunction, IFunctionAttribute>(out var result))
                return result;

            throw new Exception($"{nameof(FunctionAttribute)} is missing in the {typeof(TFunction).Name} delegate declaration." +
                                    $"Please mark the {typeof(TFunction).Name} with an appropriate {nameof(FunctionAttribute)}");
        }

        /* Override Equals & GetHashCode: ReSharper Generated */

        [ExcludeFromCodeCoverage]
        public override bool Equals(Object obj)
        {
            FunctionAttribute functionAttribute = obj as FunctionAttribute;

            if (functionAttribute == null)
                return false;
            
            return functionAttribute.Cleanup == Cleanup &&
                   functionAttribute.ReturnRegister == ReturnRegister &&
                   functionAttribute.SourceRegisters.SequenceEqual(SourceRegisters) &&
                   functionAttribute.CalleeSavedRegisters.SequenceEqual(CalleeSavedRegisters);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            int initialHash = 13;

            foreach (Register register in SourceRegisters)
                initialHash = (initialHash * 7) + (int)register;
            
            initialHash = (initialHash * 7) + (int)ReturnRegister;
            initialHash = (initialHash * 7) + (int)Cleanup;

            foreach (Register register in CalleeSavedRegisters)
                initialHash = (initialHash * 7) + (int)register;

            return initialHash;
        }
    }
}
