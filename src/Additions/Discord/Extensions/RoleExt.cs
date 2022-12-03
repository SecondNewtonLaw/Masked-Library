using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Discord.WebSocket;

namespace Masked.DiscordNet.Extensions;

public static class RoleExt
{
    /// <summary>
    /// Get all the strings to mention a collection of roles.
    /// </summary>
    /// <param name="roles">The Enumeration of roles</param>
    /// <returns>An IEnumerable of Type String</returns>
    public static async Task<IEnumerable<string>> GetMentionsAsync(this IEnumerable<SocketRole> roles)
    {
        List<SocketRole> _enumerated = roles.ToList(); // Avoid Deferring.
        string[] names = new string[_enumerated.Count];
        await Task.Run(() =>
        {
            for (int i = 0; i < _enumerated.Count; i++)
            {
                names[i] = _enumerated[i].Mention;
            }
        });
        return names;
    }
    /// <summary>
    /// Get all the strings to mention a collection of roles.
    /// </summary>
    /// <param name="roles">The Enumeration of roles</param>
    /// <returns>An IEnumerable of Type String</returns>
    public static IEnumerable<string> GetMentions(this IEnumerable<SocketRole> roles)
    {
        List<SocketRole> _enumerated = roles.ToList(); // Avoid Deferring.
        Span<string> roleNames = new string[_enumerated.Count];

        Span<SocketRole> roleSpan = CollectionsMarshal.AsSpan(_enumerated);
        ref var searchSpace = ref MemoryMarshal.GetReference(roleSpan);
        for (int i = 0; i < roleSpan.Length; i++)
        {
            var role = Unsafe.Add(ref searchSpace, i);
            roleNames[i] = role.Mention;
        }

        return roleNames.ToArray();
    }
}
