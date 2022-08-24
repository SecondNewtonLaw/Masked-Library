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
}