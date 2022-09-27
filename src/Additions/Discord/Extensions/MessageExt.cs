using Discord;

namespace Masked.DiscordNet.Extensions;

public static class MessageExt
{
    /// <summary>
    /// Deletes a message in an asynchrownous manner
    /// </summary>
    /// <param name="msg">The message socket</param>
    /// <param name="delay">The delay in milliseconds in which the message should be deleted.</param>
    /// <returns>a Task representing the operation.</returns>
    public static Task MessageDeleter<T>(T msg, int delay = 5000) where T : IMessage =>
        Task.Run(async () =>
        {
            await Task.Delay(delay);
            await msg.DeleteAsync();
        });
}