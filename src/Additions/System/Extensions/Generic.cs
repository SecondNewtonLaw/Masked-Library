using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Masked.Sys.Extensions;

public static class GenericExt
{
    /// <summary>
    /// Iterates through an array fast, very fast.
    /// </summary>
    /// <typeparam name="T">Anything that is able to be in an Array.</typeparam>
    public static void FastIterator<T>(this T[] element, Action<T, int> runInLoop)
    {
        ReadOnlySpan<T> spanOfElement = element;
        ref var searchSpace = ref MemoryMarshal.GetReference(spanOfElement);
        for (int i = 0; i < spanOfElement.Length; i++)
        {
            var obj = Unsafe.Add(ref searchSpace, i);
            runInLoop?.Invoke(obj, i);
        }
    }

    /// <summary>
    /// Iterates through a List fast, very fast.
    /// </summary>
    /// <typeparam name="T">Anything that is able to be in a List.</typeparam>
    /// <remarks>The <see cref="IEnumerable{T}"> can NOT be modified! Else this may throw an error or modify data incorrectly.</remarks>
    public static void FastIterator<T>(this IEnumerable<T> element, Action<T, int> runInLoop)
    {
        Span<T> spanOfElement = CollectionsMarshal.AsSpan(element.ToList());
        ref var searchSpace = ref MemoryMarshal.GetReference(spanOfElement);
        for (int i = 0; i < spanOfElement.Length; i++)
        {
            var obj = Unsafe.Add(ref searchSpace, i);
            runInLoop?.Invoke(obj, i);
        }
    }
}