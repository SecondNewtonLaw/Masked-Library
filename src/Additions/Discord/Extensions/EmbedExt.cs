using System.Runtime.CompilerServices;
using Discord;

namespace Masked.DiscordNet.Extensions;

public static class EmbedExt {
    /// <summary>
    /// Creates a Random color and sets it on the EmbedBuilder.
    /// </summary>
    /// <param name="embedBuilder">Instance of EmbedBuilder</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetRandomColor(this EmbedBuilder embedBuilder) {
        embedBuilder.Color = new Color(Random.Shared.Next(255), Random.Shared.Next(255), Random.Shared.Next(255));
    }
}