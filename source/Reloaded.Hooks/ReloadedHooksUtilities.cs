using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Reloaded.Hooks.Definitions;
using Reloaded.Hooks.Definitions.Helpers;
using Reloaded.Hooks.Tools;
#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
#endif

namespace Reloaded.Hooks
{
    public class ReloadedHooksUtilities : IReloadedHooksUtilities
    {
        public static ReloadedHooksUtilities Instance { get; } = new ReloadedHooksUtilities();

        public byte[] AssembleAbsoluteJump(IntPtr target, bool is64bit) => Utilities.AssembleAbsoluteJump(target.ToUnsigned(), is64bit);
        public byte[] AssemblePushReturn(IntPtr target, bool is64bit) => Utilities.AssemblePushReturn(target.ToUnsigned(), is64bit);
        public byte[] AssembleRelativeJump(IntPtr relativeJumpOffset, bool is64bit) => Utilities.AssembleRelativeJump(relativeJumpOffset, is64bit);
        public byte[] AssembleRelativeJump(IntPtr currentAddress, IntPtr targetAddress, bool is64bit) => Utilities.AssembleRelativeJump(currentAddress.ToUnsigned(), targetAddress.ToUnsigned(), is64bit);
        
        public string GetAbsoluteJumpMnemonics(IntPtr target, bool is64bit) => Utilities.GetAbsoluteJumpMnemonics(target.ToUnsigned(), is64bit);
        public string GetAbsoluteCallMnemonics(IntPtr target, bool is64bit) => Utilities.GetAbsoluteCallMnemonics(target.ToUnsigned(), is64bit);

        public string GetAbsoluteJumpMnemonics<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate => Utilities.GetAbsoluteJumpMnemonics(function, out reverseWrapper);
        public string GetAbsoluteCallMnemonics<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>(TFunction function, out IReverseWrapper<TFunction> reverseWrapper) where TFunction : Delegate => Utilities.GetAbsoluteCallMnemonics(function, out reverseWrapper);
        public string GetPushReturnMnemonics(IntPtr target, bool is64bit) => Utilities.GetPushReturnMnemonics(target.ToUnsigned(), is64bit);
        public string GetRelativeJumpMnemonics(IntPtr relativeJumpOffset, bool is64bit) => Utilities.GetRelativeJumpMnemonics(relativeJumpOffset, is64bit);

        public IntPtr InsertJump(byte[] opcodes, bool is64bit, long jumpTarget, long targetAddress = 0, long maxDisplacement = Int32.MaxValue) => Utilities.InsertJump(opcodes, is64bit, (nuint)jumpTarget.ToUnsigned(), (nuint)targetAddress, (nint)maxDisplacement).ToSigned();
        public int GetHookLength(IntPtr hookAddress, int hookLength, bool is64Bit) => Utilities.GetHookLength(hookAddress.ToUnsigned(), hookLength, is64Bit);

        public int GetNumberofParameters(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
            Type delegateType) => Utilities.GetNumberofParameters(delegateType);
        public int GetNumberofParametersWithoutFloats(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
            Type delegateType) => Utilities.GetNumberofParametersWithoutFloats(delegateType);

        public int GetNumberofParameters<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>() => Utilities.GetNumberofParameters<TFunction>();
        public int GetNumberofParametersWithoutFloats<
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.ReloadedAttributeTypes)]
#endif
        TFunction>() => Utilities.GetNumberofParametersWithoutFloats<TFunction>();

        public string PushCdeclCallerSavedRegisters() => "push eax\npush ecx\npush edx";
        public string PopCdeclCallerSavedRegisters() => "pop edx\npop ecx\npop eax";
        public IntPtr WritePointer(IntPtr target) => Utilities.WritePointer(target.ToUnsigned()).ToSigned();

        /// <inheritdoc />
        public unsafe void* GetFunctionPointer(
#if NET5_0_OR_GREATER
            [DynamicallyAccessedMembers(Trimming.Methods)]
#endif
            Type type, string name)
        {
            var method = type.GetMethod(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static).MethodHandle;
            RuntimeHelpers.PrepareMethod(method);
            return (void*) method.GetFunctionPointer();
        }

        public (long min, long max) GetRelativeJumpMinMax(long targetAddress, long maxDisplacement = Int32.MaxValue)
        {
            var result = Utilities.GetRelativeJumpMinMax((nuint)targetAddress.ToUnsigned(), (nint)maxDisplacement);
            return (result.min.ToSigned(), result.max.ToSigned());
        }
    }
}
