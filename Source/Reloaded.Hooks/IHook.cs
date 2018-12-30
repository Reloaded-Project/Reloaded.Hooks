using System;

namespace Reloaded.Hooks
{
    public interface IHook<TFunction>
    {
        TFunction OriginalFunction { get; }
        IntPtr OriginalFunctionAddress { get; }
        IReverseWrapper<TFunction> ReverseWrapper { get; }

        IHook<TFunction> Activate();

    }
}