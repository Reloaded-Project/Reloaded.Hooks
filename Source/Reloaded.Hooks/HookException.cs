using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Reloaded.Hooks
{
    [ExcludeFromCodeCoverage]
    public class HookException : Exception
    {
        public HookException() { }
        public HookException(string message) : base(message) { }
        public HookException(string message, Exception innerException) : base(message, innerException) { }
        protected HookException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
