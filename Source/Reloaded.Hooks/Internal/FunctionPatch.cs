using System.Collections.Generic;

namespace Reloaded.Hooks.Internal
{
    /// <summary>
    /// Contains a "patched" version of the start of a given native function
    /// as well as a list of patches to apply when hooking function that was "patched".
    /// </summary>
    public class FunctionPatch
    {
        public List<byte>  NewFunction;
        public List<Patch> Patches;

        public FunctionPatch()
        {
            NewFunction = new List<byte>();
            Patches  = new List<Patch>();
        }
    }
}
