


using System;

namespace Reloaded.Hooks.Definitions.Structs
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<TReturn>(FuncPtr<TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<TReturn>(void* ptr) => new FuncPtr<TReturn>(ptr);
        public static implicit operator FuncPtr<TReturn>(IntPtr ptr) => new FuncPtr<TReturn>((void*)ptr);
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Cdecl]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);
        public static implicit operator FuncPtr<TReturn>(delegate*<TReturn> ptr) => new FuncPtr<TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, TReturn>(FuncPtr<T1, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, TReturn>(void* ptr) => new FuncPtr<T1, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, TReturn>(IntPtr ptr) => new FuncPtr<T1, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, TReturn>(delegate*<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, TReturn>(FuncPtr<T1, T2, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, TReturn>(void* ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn>(FuncPtr<T1, T2, T3, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn>(FuncPtr<T1, T2, T3, T4, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn>(FuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
    }
#endif
        
#if FEATURE_FUNCTION_POINTERS
    /// <summary>
    /// Wraps a CDECL compatible function pointer.
    /// </summary>
    [X64.Function(X64.CallingConventions.Microsoft)]
    [X86.Function(X86.CallingConventions.Cdecl)]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> Invoke;

        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> ptr) { Invoke = ptr; }
        public FuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>) ptr; }

        public static implicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> func) => func.Invoke;
        public static implicit operator delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> func) => func.Invoke;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(delegate*<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(ptr);
    }
#endif
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}