using Discord;
using Discord.WebSocket;

namespace Masked.DiscordNet.Extensions;

public static class UserExt
{
    public static async Task<bool> HasRole(this SocketGuildUser user, SocketRole role)
        => await Task.Run(() => user.Roles.Contains(role));
    public static SocketGuildUser GetGuildUser<T>(this T userSocket) where T : IUser
        => (userSocket as SocketGuildUser)!;
}
