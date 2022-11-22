using Discord;

namespace Masked.DiscordNet.Extensions;

public static class EmbedExt
{
    /// <summary>
    /// Creates a Random color and sets it on the EmbedBuilder.
    /// </summary>
    /// <param name="embedBuilder">Instance of EmbedBuilder</param>
    public static void SetRandomColor(this EmbedBuilder embedBuilder)
    {
        int r = Random.Shared.Next(255);
        int g = Random.Shared.Next(255);
        int b = Random.Shared.Next(255);

        embedBuilder.Color = new Color(r, g, b);
    }
}