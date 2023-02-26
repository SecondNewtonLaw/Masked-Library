namespace Masked.SpectreConsole.Extensions;

public static class StringExtSConsole {
    /// <summary>
    /// Fixes markup syntax errors
    /// </summary>
    /// <param name="markupedString">The string with raw markup</param>
    /// <returns>a string with it's markup fixed</returns>
    public static string FixMarkup(this string markupedString) {
        return markupedString.Replace("[", "[[").Replace("]", "]]");
    }
}