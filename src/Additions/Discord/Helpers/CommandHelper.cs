using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using Masked.DiscordNet.Exceptions;
using Spectre.Console;

namespace Masked.DiscordNet;

/// <summary>
/// A Class that assists on slash command building and handling.
/// </summary>
public struct CommandHelper
{
    public CommandHelper()
    {
        Commands = new List<SlashCommandProperties>();
        CommandCode = new Dictionary<string, Func<SocketSlashCommand, Task>>();
    }
    private readonly List<SlashCommandProperties> Commands;
    private readonly Dictionary<string, Func<SocketSlashCommand, Task>> CommandCode;
    /// <summary>
    /// Build all bot commands for a specific Guild (Discord Server) [RECOMMENDED FOR TESTING ONLY]
    /// </summary>
    /// <param name="guild">Socket of the Guild.</param>
    public async Task BuildFor(SocketGuild guild)
        => await guild.BulkOverwriteApplicationCommandAsync(this.Commands.ToArray());
    /// <summary>
    /// Build all bot commands for the whole bot | Takes two hours to apply between iterations. [RECOMMENDED FOR 'PRODUCTION-READY' BOTS]
    /// </summary>
    /// <param name="client">Bot client instance</param>
    public async Task BuildApp(DiscordSocketClient client)
        => await client.BulkOverwriteGlobalApplicationCommandsAsync(this.Commands.ToArray());

    public void AddToCommandList(SlashCommandProperties commandProperties, Func<SocketSlashCommand, Task> OnCommandReceived)
    {
        // Throw error if the command has no name to pick it from.
        if (!commandProperties.Name.IsSpecified)
            throw new MissingDataException("Can not process a command if it has no name!");

        string cmdName = commandProperties.Name.Value;

        CommandCode.Add(cmdName, OnCommandReceived); // Add to Dictionary (Which will be iterated on a command received.)
        Commands.Add(commandProperties);
    }
    /// <summary>
    /// Returns a slash command handler that is able to handle all the inputted commands.
    /// </summary>
    /// <returns>An Func<T></returns>
    public Func<SocketSlashCommand, Task> GetSlashCommandHandler()
    {
        Dictionary<string, Func<SocketSlashCommand, Task>> commandCodeCopy = CommandCode;
        return async cmdSlashSocket =>
        {
            // Will check if the Dictionary contains the string of the command name, if so, it will run the Func<> it was passed.
            if (commandCodeCopy.ContainsKey(cmdSlashSocket.Data.Name))
                await commandCodeCopy[cmdSlashSocket.Data.Name].Invoke(cmdSlashSocket);
            else
                AnsiConsole.MarkupLine("[red][[Masked.DiscordNet.CommandHelper]] [marron bold underline]Warning[/]: Command '[underline italic yellow]{cmdSlashSocket.Data.Name}[/]' does not contain a valid Command Code, are you sure you have added it to the command list of the instanciated class?[/]");
        };
    }
}
