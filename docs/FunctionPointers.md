# Function Pointers

With the release of .NET 5 and C# 9.0, `Reloaded.Hooks` also additionally provides support for the usage of function pointers for those interested in maximizing performance. 

Using function pointers reduces overhead in the native <=> managed transition as we can skip delegates altogether and under the hood, use the efficient `calli` IL opcode. Outside of the quest of achieving performance however, the usage of function pointers is not recommended.

## Compromises with Function Pointers

- Static functions only.
- No compile-time checking (no compile error if your function doesn't match defined pointer).
- No marshalling.
    - Do it yourself. e.g. `Marshal.StringToHGlobalAnsi` and `Marshal.FreeHGlobal` for ANSI strings.
  
- No pointer types/ref/out (limitation of generics). 
    - You should use struct wrappers like `Reloaded.Memory`'s [BlittablePointer](https://github.com/Reloaded-Project/Reloaded.Memory/blob/master/Source/Reloaded.Memory/Pointers/BlittablePointer.cs).
    - Then in your actual `UnmanagedCallersOnly` function declaration use raw pointers. There is an implicit conversion for `BlittablePointer`.
  
- Documentation: Cannot document parameter types outside of including the info directly in the struct description.

## Defining Functions

As C# currently doesn't support named function pointers, we have to improvise a bit.

In order to define a structure, you should define a struct with a **single field** of type `FuncPtr`. The generic type arguments to the `FuncPtr` are the arguments to your function pointer and the return type.

```csharp
// Parameter 1 is `int`
// Parameter 2 is `int`
// Return type is `int`
[Function(CallingConventions.Cdecl)]
public struct CalculatorFunction { public FuncPtr<int, int, int> Value; }
```

You should then be able to use (`CalculatorFunction`) in place of the regular delegate in all common APIs.

------
Note: If no value is returned, consider using `Reloaded.Hooks.Definitions.Structs.Void` as the return parameter to help readability.

------

## Calling Functions

Calling functions is the same as with delegates, simply use the `Invoke` function of the pointer inside your struct.

```csharp
// Alias can be set at the top of the .cs file.
private CalculatorFunction _addFunctionPointer;

void makeFunctionPointer() 
{
    var addFuncPointer = ReloadedHooks.CreateWrapper<CalculatorFunction>((long)_nativeCalculator.Add, out var _);
    var three = addFuncPointer.Value.Invoke(1, 2); 
}
```

Note: There are overloads for common calling conventions `InvokeStdcall`, `InvokeCdecl` and `InvokeThiscall`. Invoke is equivalent to `InvokeStdcall`.

## Hooking Functions

Hooking functions using poitners requires .NET 5; due to the necessity of using the `UnmanagedCallersOnly` attribute.

```csharp
/* Hook object. */
private static IHook<CalculatorFunction> _addHook;

// Reloaded.Hooks assumes function pointners are `Stdcall` on Windows (.NET default).
// You should therefore use `CallConvStdcall` with your hook functions and use 
// Invoke/InvokeStdcall for calling the original function.
[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
static int AddHookFunction(int a, int b) => _addHook.OriginalFunction.Value.Invoke(a, b) + 1;

public unsafe void HookAdd()
{
    _addHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>((delegate*unmanaged[Stdcall]<int, int, int>)&AddHookFunction, (long)_nativeCalculator.Add).Activate();
}
```

For your `UnmanagedCallersOnly` function please use raw pointers in places of `BlittablePointer` (where applicable). There is an implicit conversion between the two so no manual conversions will be necessary when calling the original function again. 

There is currently an issue in the runtime where generics aren't properly checked for blittability with `UnmanagedCallersOnly`.

### Hooking via Reflection

**Experimental**: *Thorough testing not yet conducted.*

```csharp
/* Hook object. */
private static IHook<CalculatorFunction> _addHook;

[UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvStdcall) })]
static int AddHookFunction(int a, int b) => _addHook.OriginalFunction.Value.Invoke(a, b) + 1;

public unsafe void HookAdd()
{
    _addHook = ReloadedHooks.Instance.CreateHook<CalculatorFunction>(typeof(ThisClass), nameof(AddHookFunction), (long)_nativeCalculator.Add).Activate();
}
```

Writing the cast to a function pointer can be tedious.
As such, Reloaded.Hooks provides overloads which allow you to select a static function via reflection using the containing class and function name.

Note: *Local functions are not supported.*