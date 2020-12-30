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

## Benchmarks

Sample benchmarks of function hooks.

### Hardware
```ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.200-preview.20601.7
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  Job-EJHKEF : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Platform=X64  Toolchain=64 bit cli  

```

### Benchmarked Methods

The benchmark measures the time taken to execute the following function 1 million (**1,000,000**) times:

`static int Add(int a, int b) { return a + b; }`

```
|             Method | Description
|------------------- | -----------
|     ManagedInlined | Execute an inlined version of the code.
|            Managed | Execute a non-inlined version of the code.
| ManagedUnoptimized | Execute an unoptimized version of the code. (Calling Convention Compliant)
|      NoHookPointer | Execute a native assembly version of the code using C#9 function pointers (calli)
|     NoHookDelegate | Execute a native assembly version of the code using Delegates (Marshal.GetDelegateForFunctionPointer)
|       DelegateHook | Execute a hooked version using delegates.
|        FuncPtrHook | Execute a hooked version using function pointers.

```

`ManagedInlined` stack:

->.net (function)

`Managed` stack:

-> .net (implementation)\
-> .net (function)

`ManagedUnoptimized` stack:

-> .net (implementation, calling convention compliant)\
-> .net (function)

`NoHookPointer` stack:

-> native (via function pointer)\
-> .net to native transition\
-> .net (function) 

`NoHookDelegate` stack:

-> native (via function pointer)\
-> .net to native transition\
-> .net delegate code\
-> .net (function) 

`DelegateHook` stack:

-> native\
-> .net to native transition\
-> .net delegate code\
-> .net (hook function)\
-> .net delegate code\
-> native to .net transition\
-> native\
-> .net to native transition\
-> .net delegate code\
-> .net (function) 

`FuncPtrHook` stack:

-> native\
-> .net to native transition\
-> .net (hook function)\
-> native to .net transition\
-> native\
-> .net to native transition\
-> .net (function) 


### X86 Benchmarks

```markdown
|             Method |        Mean |     Error |    StdDev |
|------------------- |------------:|----------:|----------:|
|     ManagedInlined |    448.7 μs |   2.89 μs |   2.71 μs |
|            Managed |  1,754.0 μs |   9.40 μs |   7.34 μs |
| ManagedUnoptimized |  1,992.6 μs |  20.04 μs |  17.76 μs |
|      NoHookPointer |  2,661.3 μs |  16.87 μs |  14.96 μs |
|     NoHookDelegate | 10,787.7 μs | 123.28 μs | 115.32 μs |
|       DelegateHook | 41,242.1 μs | 226.90 μs | 201.14 μs |
|        FuncPtrHook | 11,825.8 μs |  54.23 μs |  48.07 μs |
```

### X64 Benchmarks

```markdown
|             Method |        Mean |     Error |    StdDev |
|------------------- |------------:|----------:|----------:|
|     ManagedInlined |    448.6 μs |   1.90 μs |   1.78 μs |
|            Managed |  1,583.8 μs |   8.50 μs |   7.53 μs |
| ManagedUnoptimized |  1,820.4 μs |  20.18 μs |  15.75 μs |
|      NoHookPointer |  6,958.4 μs |  20.41 μs |  17.04 μs |
|     NoHookDelegate | 16,784.0 μs | 111.56 μs | 104.35 μs |
|       DelegateHook | 55,987.3 μs | 456.96 μs | 405.08 μs |
|        FuncPtrHook | 24,694.2 μs | 157.92 μs | 123.29 μs |
```

### Notes

- A close estimate of the native to .net transition can be calculated with `NoHookPointer - ManagedUnoptimized`.
- Calling a function using a delegate is almost as expensive as full function hook using function pointers.
