using Newtonsoft.Json;

namespace Masked.Sys;

/// <summary>
/// A Class used to help and extend the usage of the Default Console class.
/// </summary>
public static class ConsoleExtended {
    /// <summary>
    /// When the user attempts to interrupt the Program's execution, the handler given to this method will be executed.
    /// </summary>
    /// <remarks>There may be <b>more</b> than one handler.</remarks>
    /// <param name="handler">The action ran when the Program recieves an Interrupt key combo.</param>
    public static void OnSigInt(Action<object?, ConsoleCancelEventArgs> handler) {
        Console.CancelKeyPress += (e, args) => handler?.Invoke(e, args);
    }

    /// <summary>
    /// Writes the elements of an iterable object as a list.
    /// </summary>
    /// <param name="iterable">The iterable object you desire to print.</param>
    public static void PrintIterable<T>(IEnumerable<T> iterable) {
        if (iterable is null)
            throw new ArgumentNullException(nameof(iterable));

        Console.WriteLine(string.Join(", ", iterable));
    }

    /// <summary>
    /// Writes an element as JSON to the StdOut.
    /// </summary>
    /// <param name="element">The object you want to print in a JSON-like manner.</param>
    public static void PrintAsJson<T>(T element) {
        if (element is null)
            throw new ArgumentNullException(nameof(element));

        Console.WriteLine(JsonConvert.SerializeObject(element, Formatting.Indented));
    }
}