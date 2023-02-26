using System.Runtime.Serialization;

namespace Masked.DiscordNet.Exceptions;

/// <summary>
/// An Exception class meant to throw when some important data for some part of the code is missing, meaning execution can not continue.
/// </summary>
[Serializable]
public sealed class MissingDataException : Exception {
    public MissingDataException() { }

    public MissingDataException(string message) : base(message) { }

    public MissingDataException(string message, Exception inner) : base(message, inner) { }
}