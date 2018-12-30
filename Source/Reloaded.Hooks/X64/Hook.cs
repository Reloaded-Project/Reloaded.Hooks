using System;
using System.Collections.Generic;
using System.Linq;
using Reloaded.Hooks.Internal;
using Reloaded.Hooks.Tools;
using Reloaded.Memory.Sources;
using SharpDisasm;
using static Reloaded.Memory.Sources.Memory;

namespace Reloaded.Hooks.X64
{
    /// <summary>
    /// The <see cref="Hook{TFunction}"/> class provides runtime API hooking functionality standard calling conventions
    /// as well as user defined custom conventions.
    /// </summary>
    public class Hook<TFunction> : IHook<TFunction>
    {
        /// <summary>
        /// Allows you to call the original function that was hooked.
        /// </summary>
        public TFunction OriginalFunction { get; private set; }

        /// <summary>
        /// The address to call if you wish to call the <see cref="OriginalFunction"/>.
        /// </summary>
        public IntPtr OriginalFunctionAddress { get; private set; }

        /// <summary>
        /// The reverse function wrapper that allows us to call the C# function
        /// as if it were to be of another calling convention.
        /// </summary>
        public IReverseWrapper<TFunction> ReverseWrapper { get; private set; }

        /* Patch which activates the current hook & rewrites other hooks' return addresses. */

        private Patch       _hookPatch;
        private List<Patch> _otherHookPatches;

        /// <summary>
        /// Creates a hook for a function at a given address.
        /// </summary>
        /// <param name="function">The function to detour the original function to.</param>
        /// <param name="functionAddress">The address of the function to hook.</param>
        /// <param name="minHookLength">Optional explicit length of hook. Use only in rare cases where auto-length check overflows a jmp/call opcode.</param>
        public Hook(TFunction function, long functionAddress, int minHookLength = -1)
        {
            var reverseWrapper = new ReverseWrapper<TFunction>(function);
            Create(this, reverseWrapper, functionAddress, minHookLength);
        }

        private static void Create(Hook<TFunction> hook, ReverseWrapper<TFunction> reverseWrapper, long functionAddress, int minHookLength = -1)
        {
            /* Create Convention => CDECL Wrapper. */
            List<byte> jumpOpcodes = Utilities.AssembleAbsoluteJump(reverseWrapper.WrapperPointer, true).ToList();

            /* Calculate Hook Length (Unless Explicit) */
            if (minHookLength == -1)
                minHookLength = Utilities.GetHookLength((IntPtr)functionAddress, jumpOpcodes.Count, ArchitectureMode.x86_64);

            // Sometimes our hook can be larger than the amount of bytes taken by the jmp opcode.
            // We need to fill the remaining bytes with NOPs.
            if (minHookLength > jumpOpcodes.Count)
            {
                int nopBytes = minHookLength - jumpOpcodes.Count;

                for (int x = 0; x < nopBytes; x++)
                    jumpOpcodes.Add(0x90);
            }
            
            /* Get bytes from original function prologue and patch them. */
            CurrentProcess.SafeReadRaw((IntPtr)functionAddress, out byte[] originalFunction, minHookLength);

            var functionPatcher = new FunctionPatcher(ArchitectureMode.x86_64);
            var functionPatch = functionPatcher.Patch(originalFunction.ToList(), (IntPtr)functionAddress);

            IntPtr hookEndAddress = (IntPtr)(functionAddress + minHookLength);
            functionPatch.NewFunction.AddRange(Utilities.AssembleAbsoluteJump(hookEndAddress, true));

            /* Commit the original modified function to memory. */
            byte[] patchedFunction = functionPatch.NewFunction.ToArray();
            var buffer = Utilities.FindOrCreateBufferInRange(patchedFunction.Length, 1, long.MaxValue);
            var patchedFunctionAddress = buffer.Add(patchedFunction);

            /* Create Hook instance. */
            hook.OriginalFunctionAddress = patchedFunctionAddress;
            hook.OriginalFunction  = Wrapper.Create<TFunction>((long)patchedFunctionAddress);
            hook.ReverseWrapper    = reverseWrapper;
            hook._otherHookPatches = functionPatch.Patches;
            hook._hookPatch        = new Patch((IntPtr)functionAddress, jumpOpcodes.ToArray());
        }

        /// <summary>
        /// Activates our hook.
        /// </summary>
        /// <remarks>
        ///     This function should be called after instantiation as soon as possible,
        ///     preferably in the same line as instantiation.
        ///
        ///     This class exists such that we don't run into concurrency issues on
        ///     attaching to other processes whereby the following happens:
        ///
        ///     A. Original process calls a function that was just hooked.
        ///     B. Create function has not yet returned, and OriginalFunction is unassigned.
        ///     C. Hook tried to call OriginalFunction. NullReferenceException.
        /// </remarks>
        public IHook<TFunction> Activate()
        {
            _hookPatch.Apply();

            foreach (var hookPatch in _otherHookPatches)
                hookPatch.Apply();

            return this;
        }

        /// <summary>
        /// Private constructor, constructors do not support delegates therefore use Factory Design Pattern.
        /// </summary>
        private Hook() { }
    }
}
