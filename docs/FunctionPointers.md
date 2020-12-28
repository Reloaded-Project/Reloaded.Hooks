# Function Pointers

With the release of .NET 5 and C# 9.0, Reloaded also additionally provides minimal support for the usage of function pointers. This is however a tacked on feature for now, due to limitations [some of which you can read about here](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/csharp-9.0/function-pointers#open-questions).

The main benefit of the use of function pointers is reducing overhead in the native <=> managed transition as we can skip delegates altogether and under the hood, use the efficient `calli` IL opcode.

Below are a few examples of working with pointers (taken from tests). Notably, as there are no functions to get the metadata of function pointers (yet); you still require to use delegates to define the functions in question.

## Calling Functions

Use the `CreateWrapperPtr` API in place of the `CreateWrapper` API.

```csharp
// Alias can be set at the top of the .cs file.
using FuncPtr = Reloaded.Hooks.Definitions.Structs.CdeclFuncPtr<int,int,int>;

void makeFunctionPointer() 
{
    var addFuncPointer = ReloadedHooks.CreateWrapperPtr<NativeCalculator.AddFunction, FuncPtr>((long)_nativeCalculator.Add);
    var three = addFuncPointer.Invoke(1, 2); 
}
```

## Hooking Functions

Hooking functions requires .NET 5; due to the necessity of using the `UnmanagedCallersOnly` attribute.

```csharp
// Alias can be set at the top of the .cs file.
using FuncPtr = Reloaded.Hooks.Definitions.Structs.CdeclFuncPtr<int,int,int>;

/* Hook object. */
private static IHook<NativeCalculator.AddFunction, FuncPtr> _addHook;

// The `UnmanagedCallersOnly` calling convention attribute has to match the 
// calling convention of the delegate. In .NET the default convention is Stdcall on Windows,
// therefore you should use Stdcall unless overwritten with `UnmanagedFunctionPointerAttribute`.
[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
static int AddHookFunction(int a, int b) => _addHook.OriginalFunction(a, b) + 1;

public unsafe void HookAdd()
{
    _addHook = ReloadedHooks.Instance.CreateHook<NativeCalculator.AddFunction, FuncPtr>((delegate*unmanaged[Stdcall]<int, int, int>)&AddHookFunction, (long)_nativeCalculator.Add).Activate();
}
```

Please note that on x64, only the Microsoft convention is supported for function pointers. So you should be able to freely use `CdeclFuncPtr` or `StdcallFuncPtr` interchangeably in 64-bit processes.

## Predefined Pointer Types

`Reloaded.Hooks` provides the following function pointer types out of the box:

- CdeclFuncPtr
- StdcallFuncPtr
- ThiscallFuncPtr

## Miscellaneous

### Ref/Out Parameters?

- For ref parameters at the very least, consider using an equivalent to [BlittablePointer](https://github.com/Reloaded-Project/Reloaded.Memory/blob/master/Source/Reloaded.Memory/Pointers/BlittablePointer.cs) in Reloaded.Memory; alternatively define your own struct.