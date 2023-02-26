using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Enums;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Definitions.Internal;
using Reloaded.Hooks.Definitions.X86;
using Reloaded.Hooks.Tools;
#if NET5_0_OR_GREATER
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
#endif

namespace Reloaded.Hooks
{
    public class ReloadedHooks : IReloadedHooks
    {
        public static ReloadedHooks Instance { get; } = new ReloadedHooks();

        public IFunction<TFunction> CreateFunction<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(long address) => new Function<TFunction>((nuint)address.ToUnsigned(), this);
        public IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, long functionAddress) => CreateHook<TFunction>(function, functionAddress, -1);
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(void* targetAddress, long functionAddress) => CreateHook<TFunction>(targetAddress, functionAddress, -1);
        public IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, long functionAddress, int minHookLength) => new Hook<TFunction>(function, (nuint)functionAddress.ToUnsigned(), minHookLength);
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(void* targetAddress, long functionAddress, int minHookLength) => new Hook<TFunction>(targetAddress, (nuint)functionAddress.ToUnsigned(), minHookLength);
        public IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, long functionAddress, int minHookLength, FunctionHookOptions options) => new Hook<TFunction>(function, (nuint)functionAddress.ToUnsigned(), minHookLength, options);
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(void* targetAddress, long functionAddress, int minHookLength, FunctionHookOptions options) => new Hook<TFunction>(targetAddress, (nuint)functionAddress.ToUnsigned(), minHookLength, options);

        /// <inheritdoc />
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.Methods)]
#endif
            Type type, string methodName, long functionAddress, int minHookLength) => CreateHook<TFunction>(Instance.Utilities.GetFunctionPointer(type, methodName), functionAddress, minHookLength);

        /// <inheritdoc />
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.Methods)]
#endif
            Type type, string methodName, long functionAddress) => CreateHook<TFunction>(type, methodName, functionAddress, -1);

        /// <inheritdoc />
        public unsafe IHook<TFunction> CreateHook<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.Methods)]
#endif
            Type type, string methodName, long functionAddress, int minHookLength, FunctionHookOptions options) => CreateHook<TFunction>(Instance.Utilities.GetFunctionPointer(type, methodName), functionAddress, minHookLength, options);

        public IntPtr CreateNativeWrapperX86<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(IntPtr functionAddress, IFunctionAttribute fromConvention) => X86.Wrapper.Create<TFunction>(functionAddress.ToUnsigned(), fromConvention, FunctionAttribute.GetAttribute<TFunction>().GetEquivalent(Misc.TryGetAttributeOrDefault<TFunction, UnmanagedFunctionPointerAttribute>())).ToSigned();

        public IntPtr CreateNativeWrapperX86<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(IntPtr functionAddress, IFunctionAttribute fromConvention,
            IFunctionAttribute toConvention) => X86.Wrapper.Create<TFunction>(functionAddress.ToUnsigned(), fromConvention, toConvention).ToSigned();

        public IntPtr CreateNativeWrapperX64<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(IntPtr functionAddress, Definitions.X64.IFunctionAttribute fromConvention, Definitions.X64.IFunctionAttribute toConvention) => X64.Wrapper.Create<TFunction>(functionAddress.ToUnsigned(), fromConvention, toConvention).ToSigned();

        public unsafe IntPtr CreateWrapper<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(long functionAddress)
        {
            if (sizeof(IntPtr) == 4)
                return X86.Wrapper.CreatePointer<TFunction>((nuint)functionAddress.ToUnsigned(), out _).ToSigned();

            return X64.Wrapper.CreatePointer<TFunction>((nuint)functionAddress.ToUnsigned(), out _).ToSigned();
        }

        public unsafe TFunction CreateWrapper<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(long functionAddress, out IntPtr wrapperAddress)
        {
            nuint wrapperAddr;
            if (sizeof(IntPtr) == 4)
            {
                var res86 = X86.Wrapper.Create<TFunction>((nuint)functionAddress.ToUnsigned(), out wrapperAddr);
                wrapperAddress = wrapperAddr.ToSigned();
                return res86;
            }

            var result = X64.Wrapper.Create<TFunction>((nuint)functionAddress.ToUnsigned(), out wrapperAddr);
            wrapperAddress = wrapperAddr.ToSigned();
            return result;
        }

        public unsafe IReverseWrapper<TFunction> CreateReverseWrapper<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function)
        {
            if (sizeof(IntPtr) == 4)
                return new X86.ReverseWrapper<TFunction>(function);

            return new X64.ReverseWrapper<TFunction>(function);
        }

        public unsafe IReverseWrapper<TFunction> CreateReverseWrapper<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(IntPtr function)
        {
            if (sizeof(IntPtr) == 4)
                return new X86.ReverseWrapper<TFunction>(function.ToUnsigned());

            return new X64.ReverseWrapper<TFunction>(function.ToUnsigned());
        }

        public IVirtualFunctionTable HookedVirtualFunctionTableFromObject(IntPtr objectAddress, int numberOfMethods) => HookedObjectVirtualFunctionTable.FromObject(objectAddress.ToUnsigned(), numberOfMethods);
        
        public IVirtualFunctionTable VirtualFunctionTableFromObject(IntPtr objectAddress, int numberOfMethods) => VirtualFunctionTable.FromObject(objectAddress.ToUnsigned(), numberOfMethods);
        public IVirtualFunctionTable VirtualFunctionTableFromAddress(IntPtr tableAddress, int numberOfMethods) => VirtualFunctionTable.FromAddress(tableAddress.ToUnsigned(), numberOfMethods);

        public IFunctionPtr<TDelegate> CreateFunctionPtr<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TDelegate>(ulong functionPointer) where TDelegate : Delegate => new FunctionPtr<TDelegate>(functionPointer);
        public IAsmHook CreateAsmHook(string[] asmCode, long functionAddress) => CreateAsmHook(asmCode, functionAddress, AsmHookBehaviour.ExecuteFirst, -1);
        public IAsmHook CreateAsmHook(string asmCode, long functionAddress) => CreateAsmHook(asmCode, functionAddress, AsmHookBehaviour.ExecuteFirst, -1);

        public IAsmHook CreateAsmHook(byte[] asmCode, long functionAddress) => CreateAsmHook(asmCode, functionAddress, AsmHookBehaviour.ExecuteFirst, -1);

        public IAsmHook CreateAsmHook(string[] asmCode, long functionAddress, AsmHookBehaviour behaviour) => CreateAsmHook(asmCode, functionAddress, behaviour, -1);
        public IAsmHook CreateAsmHook(string asmCode, long functionAddress, AsmHookBehaviour behaviour) => CreateAsmHook(asmCode, functionAddress, behaviour, -1);
        
        public IAsmHook CreateAsmHook(byte[] asmCode, long functionAddress, AsmHookBehaviour behaviour) => CreateAsmHook(asmCode, functionAddress, behaviour, -1);
        public IAsmHook CreateAsmHook(string asmCode, long functionAddress, AsmHookBehaviour behaviour, int hookLength) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), behaviour, hookLength);

        public IAsmHook CreateAsmHook(string[] asmCode, long functionAddress, AsmHookBehaviour behaviour, int hookLength) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), behaviour, hookLength);

        public IAsmHook CreateAsmHook(byte[] asmCode, long functionAddress, AsmHookBehaviour behaviour, int hookLength) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), behaviour, hookLength);
        public IAsmHook CreateAsmHook(string[] asmCode, long functionAddress, AsmHookOptions options) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), options);
        public IAsmHook CreateAsmHook(string asmCode, long functionAddress, AsmHookOptions options) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), options);

        public IAsmHook CreateAsmHook(byte[] asmCode, long functionAddress, AsmHookOptions options) => new AsmHook(asmCode, (nuint)functionAddress.ToUnsigned(), options);

        public IReloadedHooksUtilities Utilities { get; } = ReloadedHooksUtilities.Instance;
    }
}
