# Assembly Hooks
For advanced users requiring very specialized uses (e.g. Mid Function Hooks), a Cheat-Engine like pure assembly code hook is available.

This hook replaces the original application code with a jump to your own custom supplied code and (optionally) the original code in either of the three combinations:

- Discard Original Code
- Execute Custom Code First
- Execute Custom Code Last

## An Example
Consider the following function.

```csharp
// Returns the value of `a + b`
[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
public delegate int AddFunction(int a, int b); 
```

In native assembly, this function can be represented by something like the following:

```x86asm
// Unoptimized code, for demonstration only.
push _ebp,
mov _ebp, _esp,
mov _eax, [_ebp + wordSize * 2], // Left Parameter
mov _ecx, [_ebp + wordSize * 3], // Right Parameter
add _eax, _ecx,
pop _ebp,
ret
```

`Macros: (wordSize = 4 on x86, wordSize = 8 on x64), (_ebp is ebp on x86, _ebp is rbp on x64), etc.`

This function could, for example be manipulated to return `result + 1`, in any of the following ways.

## Discarding Original Code
Straight up replace the original code with your own.

```csharp
int wordSize = IntPtr.Size;
string[] addFunction = 
{
    $"{_use32}",
    $"push {_ebp}",
    $"mov {_ebp}, {_esp}",

    $"mov {_eax}, [{_ebp} + {wordSize * 2}]", // Left Parameter
    $"mov {_ecx}, [{_ebp} + {wordSize * 3}]", // Right Parameter
    $"add {_eax}, 1",                         // Left Parameter
};

_addNoOriginalHook = new AsmHook(addFunction, addressOfNativeCode, AsmHookBehaviour.DoNotExecuteOriginal).Activate();
```

## Executing Original Code First
```csharp
string[] addFunction =
{
    $"{_use32}",
    $"add {_eax}, 1", // Left Parameter - Should have already been copied from stack.
};

_addAfterOriginalHook = new AsmHook(addFunction, addressOfNativeCode, AsmHookBehaviour.ExecuteAfter).Activate();
```

## Executing Original Code Last
```csharp
int wordSize = IntPtr.Size;
string[] addFunction =
{
    $"{_use32}",
    $"add [{_esp} + {wordSize * 1}], byte 1", // Add 1 to left parameter on stack.
};

_addBeforeOriginalHook = new AsmHook(addFunction, addressOfNativeCode, AsmHookBehaviour.ExecuteFirst).Activate();
```

## What's "Original Code" ?

**Answer:** *The minimal amount of x86/64 assembly instructions required to use 7 or more bytes.*

Understanding how this hook works helps a bit. Essentially what this hook does is place a `jmp` instruction at the location supplied as the `addressOfNativeCode` to your asm code. However, as the jump to your code overwrites the original code, something has to be done with that original code.

What this hook does is it calculates the minimal amount of assembly instructions (starting from `addressOfNativeCode`) necessary to free up space for the `jmp` instruction. That is the minimal amount of instructions that uses >= 7 bytes of space. Those instructions are referred to as the "original code". Depending on your preference, this code is either discarded or inserted before/after your own code.

In other words; before you use this hook, you have to look at your disassembly and determine the minimum amount of instructions what take >- 7 bytes. When `Reloaded.Hooks` does its work; that will be the "original code". As such, this hook is reserved for advanced users.

(Note: A bit more actually happens under the hood to enable the `Disable/Enable` toggle, but this is the simple version).