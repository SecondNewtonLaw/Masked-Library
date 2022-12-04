using Discord;
using Discord.WebSocket;

namespace Masked.DiscordNet;

/// <summary>
/// Interface used to implement Discord Bot commands. Used by <see cref="CommandHelper"/>
/// </summary>
public interface IDiscordCommand
{
    /// <summary>
    /// Method used to Run the logic of the Command.
    /// </summary>
    /// <param name="commandSocket">The socket used to interact with the Discord Interaction.</param>
    /// <returns>A Task representing the on-going asynchronous operation.</returns>
    public static abstract Task Run(SocketSlashCommand commandSocket);
    /// <summary>
    /// Method used to Build the properties of the Slash Command, which are sent to Discord.
    /// </summary>
    /// <returns>The Properties of the Slash Command.</returns>
    public static abstract SlashCommandProperties Build();
}