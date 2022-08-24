using System.Text;

namespace Masked.Sys.Extensions;

public static class StringExt
{
    /// <summary>
    /// Assembles the current <see cref="char"/> enumeration into a <see cref="string"/>
    /// </summary>
    /// <param name="chars">The collcection of charaters to aseemble into a string</param>
    /// <returns>a string containing the collection of characters as a string</returns>
    public static string AssembleToString<Type>(this Type chars) where Type : IEnumerable<char>
        => string.Join("", chars);

    /// <summary>
    /// Removes characters from a string asynchronously
    /// </summary>
    /// <returns>a new string without illegal characters in it.</returns>
    public static async Task<string> SanitizeString(this string fileName, char[] illegalCharacters, CancellationToken tken = new())
    {
        char[] stringChars = fileName.ToCharArray();
        StringBuilder result = new();

        result.Append((await Task.Factory.StartNew(
                () =>
                stringChars.Where(
                    x => illegalCharacters.Contains(x)
                    ),
                    tken,
                    TaskCreationOptions.PreferFairness,
                    TaskScheduler.Default
                )).ToArray());

        return result.ToString();
    }
}