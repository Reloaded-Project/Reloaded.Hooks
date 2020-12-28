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
        public static bool TryGetAttribute<TType, TAttribute>(out TAttribute result)
        {
            result = default;
            foreach (Attribute attribute in typeof(TType).GetCustomAttributes(true))
            {
                if (attribute is TAttribute target)
                {
                    result = target;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to find a given attribute for a given generic type.
        /// </summary>
        public static TAttribute TryGetAttributeOrDefault<TType, TAttribute>()
        {
            if (TryGetAttribute<TType, TAttribute>(out var result))
                return result;

            return default;
        }
    }
}
