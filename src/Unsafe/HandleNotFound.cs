using System.Runtime.Serialization;

namespace Masked.Sys.Dangerous.Unsafe;

[Serializable]
internal class HandleNotFound : Exception {
    public HandleNotFound() { }

    public HandleNotFound(string? message) : base(message) { }

    public HandleNotFound(string? message, Exception? innerException) : base(message, innerException) { }

    protected HandleNotFound(SerializationInfo info, StreamingContext context) : base(info, context) { }
}