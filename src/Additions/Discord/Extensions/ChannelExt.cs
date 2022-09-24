using Discord.WebSocket;

namespace Masked.DiscordNet.Extensions;

public static class ChannelExt
{
    /// <summary>
    /// Obtains the Guild of a Channel
    /// </summary>
    /// <typeparam name="T">Channel Type where ISocketMessageChannel</typeparam>
    /// <param name="channelSocket">The Channel you want to gain the guild of</param>
    /// <returns>The Guild of the channel</returns>
    /// <exception cref="InvalidOperationException">Thrown if the Channel does not have a guild in their information</exception>
    public static SocketGuild GetGuild<T>(this T channelSocket) where T : ISocketMessageChannel
    {
        try
        {
            return channelSocket is not SocketGuildChannel gChan
                ? throw new InvalidOperationException($"Channel with ID {channelSocket.Id} does not point to a valid guild! Are you sure its origin is from a guiid?")
                : gChan.Guild;
        }
        catch
        {
            throw;
        }
    }
}