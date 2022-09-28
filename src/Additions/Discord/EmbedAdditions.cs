using Discord;

namespace Masked.DiscordNet;

public sealed class EmbedAdditions
{
    public static EmbedFooterBuilder GetGMTFooter()
        => new() { Text = $"GMT Time: {DateTime.UtcNow}" };

    public static EmbedFooterBuilder GetLocalTimeFooter()
        => new() { Text = $"Local Time: {DateTime.Now}" };

    public static EmbedAuthor CreateEmbedAuthor(string authorName, Uri authorIcon, Uri authorUrl) => new EmbedAuthorBuilder()
    {
        Name = authorName,
        Url = authorUrl.AbsoluteUri,
        IconUrl = authorIcon.AbsoluteUri,
    }.Build();
}