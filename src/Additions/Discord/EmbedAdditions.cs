using Discord;

namespace Masked.DiscordNet;

public sealed class EmbedAdditions {
    public static EmbedFooterBuilder GetGMTFooter() {
        return new() { Text = $"GMT Time: {DateTime.UtcNow}" };
    }

    public static EmbedFooterBuilder GetLocalTimeFooter() {
        return new() { Text = $"Local Time: {DateTime.Now}" };
    }

    public static EmbedAuthor CreateEmbedAuthor(string authorName, Uri authorIcon, Uri authorUrl) {
        return new EmbedAuthorBuilder() {
            Name = authorName,
            Url = authorUrl.AbsoluteUri,
            IconUrl = authorIcon.AbsoluteUri
        }.Build();
    }
}