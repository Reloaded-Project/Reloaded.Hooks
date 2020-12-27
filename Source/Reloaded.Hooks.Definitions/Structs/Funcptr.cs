


using System;

namespace Reloaded.Hooks.Definitions.Structs
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        
    public unsafe struct CdeclFuncPtr<TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<TReturn>(void* ptr) => new CdeclFuncPtr<TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<TReturn>(IntPtr ptr) => new CdeclFuncPtr<TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<TReturn>(delegate*unmanaged[Cdecl]<TReturn> ptr) => new CdeclFuncPtr<TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<TReturn>(void* ptr) => new StdcallFuncPtr<TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<TReturn>(IntPtr ptr) => new StdcallFuncPtr<TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<TReturn>(delegate*unmanaged[Stdcall]<TReturn> ptr) => new StdcallFuncPtr<TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<TReturn>(void* ptr) => new ThiscallFuncPtr<TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<TReturn>(IntPtr ptr) => new ThiscallFuncPtr<TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<TReturn>(delegate*unmanaged[Thiscall]<TReturn> ptr) => new ThiscallFuncPtr<TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, TReturn>(void* ptr) => new CdeclFuncPtr<T1, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, TReturn>(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) => new CdeclFuncPtr<T1, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, TReturn>(void* ptr) => new StdcallFuncPtr<T1, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, TReturn>(delegate*unmanaged[Stdcall]<T1, TReturn> ptr) => new StdcallFuncPtr<T1, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, TReturn>(delegate*unmanaged[Thiscall]<T1, TReturn> ptr) => new ThiscallFuncPtr<T1, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) => new CdeclFuncPtr<T1, T2, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, TReturn> ptr) => new StdcallFuncPtr<T1, T2, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, T4, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, T4, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, T4, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, T4, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, T4, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, T4, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, T4, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, T4, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
    }
        
    public unsafe struct CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Invoke;

        public CdeclFuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { Invoke = ptr; }
        public CdeclFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>) ptr; }

        public static implicit operator void*(CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.Invoke;
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(void* ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(IntPtr ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>((void*)ptr);
        public static implicit operator CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new CdeclFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
    }
        
    public unsafe struct StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Invoke;

        public StdcallFuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { Invoke = ptr; }
        public StdcallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>) ptr; }

        public static implicit operator void*(StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.Invoke;
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(void* ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(IntPtr ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>((void*)ptr);
        public static implicit operator StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new StdcallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
    }
        
    public unsafe struct ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>
    {
        /// <summary>
        /// Calls the underlying function.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Invoke;

        public ThiscallFuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { Invoke = ptr; }
        public ThiscallFuncPtr(void* ptr) { Invoke = (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>) ptr; }

        public static implicit operator void*(ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.Invoke;
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(void* ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(IntPtr ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>((void*)ptr);
        public static implicit operator ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new ThiscallFuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}