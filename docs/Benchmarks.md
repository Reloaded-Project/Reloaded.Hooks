# Benchmarks

Sample benchmarks of function hooks.

## Hardware
```ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
Intel Core i7-4790K CPU 4.00GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=5.0.200-preview.20601.7
  [Host]     : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  Job-EJHKEF : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Platform=X64  Toolchain=64 bit cli  

```

## Benchmarked Methods

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

- .net (function)

`Managed` stack:

- .net (implementation)
- .net (function)

`ManagedUnoptimized` stack:

- .net (implementation, calling convention compliant)
- .net (function)

`NoHookPointer` stack:

- native (via function pointer)
- .net to native transition
- .net (function) 

`NoHookDelegate` stack:

- native (via function pointer)
- .net to native transition
- .net delegate code
- .net (function) 

`DelegateHook` stack:

- native
- .net to native transition
- .net delegate code
- .net (hook function)
- .net delegate code
- native to .net transition
- native
- .net to native transition
- .net delegate code
- .net (function) 

`FuncPtrHook` stack:

- native
- .net to native transition
- .net (hook function)
- native to .net transition
- native
- .net to native transition
- .net (function) 


## X86 Benchmarks

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

## X64 Benchmarks

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

## Notes

- A close estimate of the native to .net transition can be calculated with `NoHookPointer - ManagedUnoptimized`.
- Calling a function using a delegate is almost as expensive as full function hook using function pointers.
- If absolute performance is a must (e.g. hooking graphics APIs called 100,000+ times per second), you should probably use native code instead.
- Some improved performance may be seen in the future based on a possible implementation of [CallConvSuppressGCTransition](https://github.com/dotnet/runtime/issues/38134).
