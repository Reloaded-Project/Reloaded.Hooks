using System;

namespace Reloaded.Hooks
{
    public interface IReverseWrapper<TFunction>
    {
        TFunction CSharpFunction { get; }
        IntPtr NativeFunctionPtr { get; }
        IntPtr WrapperPointer { get; }
    }
}