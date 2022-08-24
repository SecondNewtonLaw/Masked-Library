using Discord.WebSocket;

namespace Masked.DiscordNet.Extensions;
public static class RolesExt
{
    /// <summary>
    /// Get all the strings to mention a collection of roles.
    /// </summary>
    /// <param name="roles">The collction of roles</param>
    /// <returns>An ICollection of Type String</returns>
    public static async Task<ICollection<string>> GetMentions(this ICollection<SocketRole> roles)
    {
        List<string> names = new();
        await Task.Run(
        () =>
        {
            for (int i = 0; i < roles.Count; i++)
            {
                names.Add(roles.ElementAt(i).Mention);
            }
        });
        return names;
    }
}