using System;
using System.Linq;

namespace Reloaded.Hooks.Definitions.Structs
{
    /// <summary>
    /// Interface implemented by all function pointers.
    /// </summary>
    public interface IFuncPtr
    {
        /// <summary>
        /// Gets the number of generic parameters.
        /// </summary>
        public int NumberOfParameters { get; }

        /// <summary>
        /// Gets the number of generic parameters, excluding float numbers.
        /// </summary>
        public int NumberOfParametersWithoutFloats { get; }

    };

    internal static class FuncPtr
    {
        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public static int GetNumberOfParameters(Type type) => type.GenericTypeArguments.Length - 1;

        /// <summary>
        /// Gets the number of parameters.
        /// </summary>
        public static int GetNumberOfParametersWithoutFloats(Type type) => type.GenericTypeArguments.Take(GetNumberOfParameters(type)).Count(x => x != typeof(Single) && x != typeof(Double));
    }
}