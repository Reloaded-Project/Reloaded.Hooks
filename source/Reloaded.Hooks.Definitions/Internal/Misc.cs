using System;

namespace Reloaded.Hooks.Definitions.Internal
{
    /// <summary>
    /// Miscellaneous Functions
    /// </summary>
    public class Misc
    {
        /// <summary>
        /// Attempts to find a given attribute for a given generic type
        /// </summary>
        public static bool TryGetAttribute<TType, TAttribute>(out TAttribute result) where TAttribute : class
        {
            result = default;
            foreach (Attribute attribute in typeof(TType).GetCustomAttributes(false))
            {
                if (attribute.GetType() == typeof(TAttribute))
                {
                    result = attribute as TAttribute;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to find a given attribute for a given generic type.
        /// </summary>
        public static TAttribute TryGetAttributeOrDefault<TType, TAttribute>() where TAttribute : class
        {
            if (TryGetAttribute<TType, TAttribute>(out var result))
                return result;

            return default;
        }
    }
}
