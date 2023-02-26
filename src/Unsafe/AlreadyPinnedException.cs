using System.Runtime.Serialization;

namespace Masked.Sys.Dangerous.Unsafe;

[Serializable]
internal class AlreadyPinnedException : Exception {
    public AlreadyPinnedException() { }

    public AlreadyPinnedException(string? message) : base(message) { }

    public AlreadyPinnedException(string? message, Exception? innerException) : base(message, innerException) { }

    protected AlreadyPinnedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}