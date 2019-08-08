using System;
using System.Diagnostics;
using System.Threading;

namespace Reloaded.Hooks.Internal
{
    public class Mutex
    {
        public static System.Threading.Mutex MakeHookMutex { get; }
        public static System.Threading.Mutex MakeWrapperMutex { get; }
        public static System.Threading.Mutex MakeReverseWrapperMutex { get; }
        public static System.Threading.Mutex FunctionPatcherMutex { get; }

        // Creates a name for a specified mutex type.
        private static string GetMutexName(String mutexType, Process process) => $"Reloaded.Hooks | {mutexType} | PID: {process.Id}";

        static Mutex()
        {
            MakeHookMutex = MakeMutex(GetMutexName("Make Hook", Process.GetCurrentProcess()));
            MakeWrapperMutex = MakeMutex(GetMutexName("Make Wrapper", Process.GetCurrentProcess()));
            MakeReverseWrapperMutex = MakeMutex(GetMutexName("Make Reverse Wrapper", Process.GetCurrentProcess()));
            FunctionPatcherMutex = MakeMutex(GetMutexName("Function Patcher", Process.GetCurrentProcess()));
        }

        /// <summary>
        /// Opens an existing mutex with a specified name if exists. Otherwise creates a new Mutex.
        /// </summary>
        /// <param name="mutexName">The name of the mutex.</param>
        /// <returns>An instance of the Mutex111</returns>
        private static System.Threading.Mutex MakeMutex(String mutexName)
        {
            try
            {
                return System.Threading.Mutex.OpenExisting(mutexName);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                return new System.Threading.Mutex(false, mutexName);
            }
        }
    }
}
