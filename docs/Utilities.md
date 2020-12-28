# Utilities

This section is a quick reference/directory to the various utility functions and classes available.

##  ReloadedHooks & IReloadedHooks

`ReloadedHooks` is a one stop shop for this library, encompassing all main and commonly used functionality inside of a single class.

For example:

```csharp
_reloadedHooks.CreateWrapper<NtCreateFile>(ntCreateFile, out _);
```

Would be functionally equivalent to: `new X86.Wrapper.Create<TFunction>(function, out _);` or  `new X64.Wrapper.Create<TFunction>(function, out _);` depending on architecture of current process.

If you are already familiar with `Reloaded.Hooks` as a library, everything inside `IReloadedHooks` should be self explanatory.

##  Function<T>

`Function<T>` is an API that wraps a native function given a pointer and instance of `IReloadedHooks`. 

For example, creating an instance of the class:

```csharp
// _calculator.Add is a function address
// _hooks is an instance of ReloadedHooks
_addFunction = new Function<Calculator.AddFunction>((long)_calculator.Add, _hooks);
```

Allows you to more easily use common operations of the library.

```csharp
// Hook Function
_addHook = _addFunction.Hook(Hookfunction).Activate();

// Call Function
 _addFunction.GetWrapper()(x, y);
```

The intended use of this class is to simplify usage when building APIs that can interface with a given process. For example: APIs that allow for hacking specific games.

**Remarks:**
The return value of `GetWrapper()` is cached, repeated calls will return the same results.

## Calling Function Pointers

Should you ever find yourself needing to call functions that are pointed to by a pointer whose value constantly changes, `Reloaded.Hooks` provides you with a simple utility class to help you alleviate the pain of constantly changing function addresses.

The utility class is simply named `FunctionPtr` and can be found at `Reloaded.Hooks.Tools` respectively - the usage is simple. Here is a small sample example:

```csharp
// 0x123456 is the address of a pointer which points to a function of type MyCustomDelegate
// MyCustomDelegate is a delegate marked with FunctionAttribute and UnmanagedFunctionPointerAttribute.
FunctionPtr<MyCustomDelegate> functionPtr = new FunctionPtr<MyCustomDelegate>(0x123456);

// Gets the address of our function (dereferences pointer).
var functionAddress = functionPtr.GetFunctionAddress(0); // Index: This class supports arrays of pointers.

// Gets the delegate to use for calling the native function and calls the function.
// You should this every time you intend to use the function pointer to call function. 
var myCustomFunction = functionPtr.GetDelegate();
myCustomFunction(1000);
```