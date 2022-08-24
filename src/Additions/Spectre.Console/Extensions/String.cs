namespace Masked.SpectreConsole.Extensions;

public static class StringExtSConsole
{

    /// <summary>
    /// Fixes markup syntax errors
    /// </summary>
    /// <param name="markupeddString">The string with raw markup</param>
    /// /// <returns>a string with it's markup fixed</returns>
    public static string FixMarkup(this string markupedString) => markupedString.Replace("[", "[[").Replace("]", "]]");

}