


using System;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace Reloaded.Hooks.Definitions.Structs
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<TReturn> Invoke => (delegate*unmanaged[Stdcall]<TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<TReturn>(void* ptr) => new FuncPtr<TReturn>(ptr);
        public static implicit operator FuncPtr<TReturn>(IntPtr ptr) => new FuncPtr<TReturn>((void*)ptr);
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Cdecl]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Stdcall]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Thiscall]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<TReturn> ptr) => new FuncPtr<TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, TReturn>(void* ptr) => new FuncPtr<T1, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, TReturn>(IntPtr ptr) => new FuncPtr<T1, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Cdecl]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Stdcall]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Thiscall]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, TReturn> ptr) => new FuncPtr<T1, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, TReturn>(void* ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, TReturn> ptr) => new FuncPtr<T1, T2, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, TReturn> ptr) => new FuncPtr<T1, T2, T3, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>));
    }

    [ExcludeFromCodeCoverage]
    public unsafe struct FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> : IFuncPtr
    {
        /// <summary>
        /// Raw pointer to the function.
        /// </summary>
        public void* funcPtr;

    
        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> Invoke => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;

        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Cdecl convention.
        /// </summary>
        public delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsCdecl => (delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Cdecl convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsCdeclDangerous => (delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Stdcall convention.
        /// </summary>
        public delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsStdcall => (delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Stdcall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsStdcallDangerous => (delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        #endif

        /// <summary>
        /// Calls the underlying function using the Thiscall convention.
        /// </summary>
        public delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsThiscall => (delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        
        #if NET6_0_OR_GREATER
        /// <summary>
        /// Calls the underlying function using the Thiscall convention using the SuppressGCTransition attribute.
        /// Please read: https://docs.microsoft.com/en-us/dotnet/api/system.runtime.interopservices.suppressgctransitionattribute
        /// </summary>
        public delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> InvokeAsThiscallDangerous => (delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>)funcPtr;
        #endif

        public FuncPtr(void* ptr) { funcPtr = ptr; }
        public FuncPtr(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #endif
        public FuncPtr(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #if NET6_0_OR_GREATER
        public FuncPtr(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) { funcPtr = ptr; }
        #endif

        public static explicit operator void*(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> func) => func.funcPtr;
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(void* ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(IntPtr ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>((void*)ptr);
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Cdecl]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Cdecl, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Stdcall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Stdcall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        #endif
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Thiscall]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);

        #if NET6_0_OR_GREATER
        public static implicit operator FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(delegate*unmanaged[Thiscall, SuppressGCTransition]<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn> ptr) => new FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ptr);
        #endif

        /// <inheritdoc />
        public int NumberOfParameters => FuncPtr.GetNumberOfParameters(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>));

        /// <inheritdoc />
        public int NumberOfParametersWithoutFloats => FuncPtr.GetNumberOfParametersWithoutFloats(typeof(FuncPtr<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>));
    }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}