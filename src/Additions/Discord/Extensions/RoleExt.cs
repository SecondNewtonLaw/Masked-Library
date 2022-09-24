using Discord.WebSocket;

namespace Masked.DiscordNet.Extensions;

public static class RoleExt
{
    /// <summary>
    /// Get all the strings to mention a collection of roles.
    /// </summary>
    /// <param name="roles">The collction of roles</param>
    /// <returns>An ICollection of Type String</returns>
    public static async Task<ICollection<string>> GetMentions(this ICollection<SocketRole> roles)
    {
        string[] names = new string[roles.Count];
        await Task.Run(() =>
        {
            for (int i = 0; i < roles.Count; i++)
            {
                names[i] = roles.ElementAt(i).Mention;
            }
        });
        return names;
    }
}