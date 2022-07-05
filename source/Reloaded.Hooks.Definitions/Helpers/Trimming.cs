#if NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using static System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes;
#pragma warning disable CS1591

namespace Reloaded.Hooks.Definitions.Helpers;

public class Trimming
{
    public const DynamicallyAccessedMemberTypes FuncPtrTypes = PublicParameterlessConstructor | PublicFields | PublicNestedTypes;
    public const DynamicallyAccessedMemberTypes ReloadedAttributeTypes = FuncPtrTypes | PublicMethods | NonPublicMethods;
    public const DynamicallyAccessedMemberTypes Methods = PublicMethods | NonPublicMethods;
}
#endif