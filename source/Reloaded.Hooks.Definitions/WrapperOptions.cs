#nullable enable
using Reloaded.Hooks.Definitions.Structs;

namespace Reloaded.Hooks.Definitions;

/// <summary>
/// Options for wrapper generation.
/// </summary>
public struct WrapperOptions
{
    /// <summary>
    /// If specified, this address represents a target near which the wrapper should be allocated.  
    /// </summary>
    public ProximityTarget? ProximityTarget { get; set; }

    /// <summary>
    /// Creates an instance of <see cref="WrapperOptions"/> with a given proximity target.
    /// </summary>
    public WrapperOptions() => ProximityTarget = null;

    /// <summary>
    /// Creates an instance of <see cref="WrapperOptions"/> with a given proximity target.
    /// </summary>
    /// <param name="proximityTarget">The target to allocate wrapper near.</param>
    public WrapperOptions(ProximityTarget proximityTarget) => ProximityTarget = proximityTarget;
}