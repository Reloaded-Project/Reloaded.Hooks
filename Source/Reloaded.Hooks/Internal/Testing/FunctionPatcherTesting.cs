using System.Collections.Generic;

namespace Reloaded.Hooks.Internal.Testing
{
    /// <summary>
    /// [TESTING USE ONLY]
    /// </summary>
    public static class FunctionPatcherTesting
    {
        /// <summary>
        /// [TESTING USE ONLY]
        /// </summary>
        public static List<Patch> PatchJumpTargets(FunctionPatcher patcher, AddressRange searchRange, AddressRange originalJmpTarget, long newJmpTarget)
        {
            return patcher.PatchJumpTargets(searchRange, originalJmpTarget, newJmpTarget);
        }

        /// <summary>
        /// [TESTING USE ONLY]
        /// </summary>
        public static void GetSearchRange(FunctionPatcher patcher, ref long searchPointer, out long searchLength)
        {
            patcher.GetSearchRange(ref searchPointer, out searchLength);
        }
    }
}
