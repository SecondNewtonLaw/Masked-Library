using System.Text;

namespace Masked.Sys.Extensions;

/// <summary>
/// Extensions for the string class.
/// </summary>
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

        _ = result.Append((await Task.Factory.StartNew(
                () =>
                stringChars.Where(
                    x => illegalCharacters.Contains(x)
                    ),
                    tken,
                    TaskCreationOptions.PreferFairness,
                    TaskScheduler.Default
                ).ConfigureAwait(false)).ToArray());

        return result.ToString();
    }

    /// <summary>
    /// Checks a string in search of characters in it.
    /// </summary>
    /// <param name="str">The string instance.</param>
    /// <param name="chars">The collection of characters that are to be checked for.</param>
    /// <returns>A True if the string contains any of the characters in the collection</returns>
    public static Task<bool> Contains(this string str, ICollection<char> chars)
    {
        Task<bool> Assertion = new(() =>
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (chars.Contains(str[i]))
                    return true;
            }

            return false;
        });
        Assertion.Start();
        return Assertion;
    }

    /// <summary>
    /// Checks an array of strings in search a string in it.
    /// </summary>
    /// <param name="strArr">The string[] instance.</param>
    /// <param name="target">The string that should be checked for.</param>
    /// <param name="comparer">The comparer that should be used when comparing the strings.</param>
    /// <returns>A True if the string[] contains the string</returns>
    public static Task<bool> Contains(this string[] strArr, string target, StringComparison comparer = StringComparison.CurrentCulture)
    {
        Task<bool> Assertion = new(() =>
        {
            for (int i = 0; i < strArr.Length; i++)
            {
                if (strArr[i].Contains(target, comparer))
                    return true;
            }

            return false;
        });
        Assertion.Start();
        return Assertion;
    }

    /// <summary>
    /// Verify wether or not the string is a Base64 string
    /// </summary>
    /// <param name="str">String Instance</param>
    /// <returns>True if it is a Base64 string</returns>
    public static Task<bool> IsBase64(this string str)
        => Task.Run(() => Convert.TryFromBase64String(str, new(), out _));
}