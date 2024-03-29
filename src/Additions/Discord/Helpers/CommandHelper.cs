using Discord;
using Discord.Rest;
using Discord.WebSocket;
using Masked.DiscordNet.Exceptions;
using Masked.DiscordNet.Extensions;
using Spectre.Console;

namespace Masked.DiscordNet;

/// <summary>
/// A Class that assists on slash command building and handling.
/// </summary>
public sealed class CommandHelper {
    public CommandHelper() {
        Commands = new List<SlashCommandProperties>();
        CommandCode = new Dictionary<string, Func<SocketSlashCommand, Task>>(StringComparer.Ordinal);
    }

    private readonly List<SlashCommandProperties> Commands;
    private readonly Dictionary<string, Func<SocketSlashCommand, Task>> CommandCode;

    /// <summary>
    /// Build all bot commands for a specific Guild (Discord Server) [RECOMMENDED FOR TESTING ONLY]
    /// </summary>
    /// <param name="guild">Socket of the Guild.</param>
    public Task BuildFor(SocketGuild guild) {
        return guild.BulkOverwriteApplicationCommandAsync(Commands.ToArray());
    }

    /// <summary>
    /// Build all bot commands for the whole bot | Takes two hours to apply between iterations. [RECOMMENDED FOR 'PRODUCTION-READY' BOTS]
    /// </summary>
    /// <param name="client">Bot client instance</param>
    public Task BuildApp(DiscordSocketClient client) {
        return client.BulkOverwriteGlobalApplicationCommandsAsync(Commands.ToArray());
    }

    /// <summary>
    /// Add a command to command builder
    /// </summary>
    /// <param name="commandProperties">THe properties of the command</param>
    /// <param name="OnCommandReceived">What to do when the command is received by the bot</param>
    /// <exception cref="MissingDataException">Thrown if the Properties of the Slash Command is missing it's Name, making it unable to be identified when received</exception>
    [Obsolete("This method is obsolete, please use the AddCommandToList(IDiscordCommand) method overload instead.")]
    public void AddToCommandList(SlashCommandProperties commandProperties,
        Func<SocketSlashCommand, Task> OnCommandReceived) {
        // Throw error if the command has no name to pick it from.
        if (!commandProperties.Name.IsSpecified)
            throw new MissingDataException("Can not process a command if it has no name!");

        var cmdName = commandProperties.Name.Value;

        CommandCode.Add(cmdName,
            OnCommandReceived); // Add to Dictionary (Which will be iterated on a command received.)
        Commands.Add(commandProperties);
    }

    /// <summary>
    /// Add a command to command builder
    /// </summary>
    /// <param name="command">A class that implements the <see cref="IDiscordCommand"/> interface.</param>
    /// <exception cref="MissingDataException">Thrown if the Properties of the Slash Command is missing it's Name, making it unable to be identified when received</exception>
    public void AddToCommandList(IDiscordCommand command) {
        var commandProperties = command.Build();

        async Task OnCommandReceived(SocketSlashCommand sock) {
            await command.Run(sock);
        }

        // Throw error if the command has no name to pick it from.
        if (!commandProperties.Name.IsSpecified)
            throw new MissingDataException("Can not process a command if it has no name!");

        var cmdName = commandProperties.Name.Value;

        CommandCode.Add(cmdName,
            OnCommandReceived); // Add to Dictionary (Which will be iterated on a command received.)
        Commands.Add(commandProperties);
    }

    /// <summary>
    /// Returns a slash command handler that is able to handle all the inputted commands.
    /// </summary>
    /// <returns>An Func that holds logic to process each command upon receiving it</returns>
    public Func<SocketSlashCommand, Task> GetSlashCommandHandler() {
        Dictionary<string, Func<SocketSlashCommand, Task>> commandCodeCopy = CommandCode;
        return async cmdSlashSocket => {
            // Will check if the Dictionary contains the string of the command name, if so, it will run the Func<> it was passed.
            if (commandCodeCopy.TryGetValue(cmdSlashSocket.Data.Name,
                    out var commandInvocation))
                await commandInvocation.Invoke(cmdSlashSocket).ConfigureAwait(false);
            else
                AnsiConsole.MarkupLine(
                    "[red][[Masked.DiscordNet.CommandHelper]] [marron bold underline]Warning[/]: Command '[underline italic yellow]{cmdSlashSocket.Data.Name}[/]' does not contain a valid Command Code, are you sure you have added it to the command list of the instanciated class?[/]");
        };
    }

    /// <summary>
    /// Submits a command that triggers a command building for the guild the command was executed in.
    /// The Handler received from GetSlashCommandHandler() MUST be set
    /// If the command already exists remotely, it will register it to the command handler.
    /// </summary>
    /// <returns>A Task representing the running operation.</returns>
    public async Task SubmitCommandBuilder(SocketGuild guild) {
        SlashCommandBuilder buildCommand = new() {
            Name = "buildcommands",
            Description = "A command used to build the other commands of the bot"
        };

        async Task buildCommandLogic(SocketSlashCommand sock) {
            await sock.DeferAsync().ConfigureAwait(false);
            var msg = await sock.FollowupAsync("**[Dev command]** `Building slash commands for this guild remotely...`")
                .ConfigureAwait(false);

            var guild = sock.Channel.GetGuild();
            await BuildFor(guild).ConfigureAwait(false);

            await msg.ModifyAsync(x => x.Content = "**[Dev Command]** `Commands Built.`").ConfigureAwait(false);
            // End.
        }

        var buildCmdProps = buildCommand.Build();
        var innerCmdName = buildCommand.Name;
        
        if (!Commands.Contains(buildCmdProps))
            // Ignore obsolescence. too lazy to move to new code.
            AddToCommandList(buildCmdProps, buildCommandLogic);

        // Only submit IF it doesn't exist in the server already
        if (!(await guild.GetApplicationCommandsAsync().ConfigureAwait(false)).Any(x => string.Equals(
                x.Name,
                innerCmdName,
                StringComparison.InvariantCulture)))
            _ = await guild.CreateApplicationCommandAsync(buildCmdProps).ConfigureAwait(false);
    }

    /// <summary>
    /// Submits the SlashCommandProperties that are managed by this class instance to the Discord API.
    /// </summary>
    /// <param name="client">The Discord Client that will be used to submit the commands.</param>
    /// <remarks>This method will ALSO assign the SlashCommandHandler required for this operation after the Commands have been submitted.</remarks>
    /// <returns>A Task representing the on-going asynchronous operation.</returns>
    public async Task SubmitCommands(DiscordSocketClient client) {
        AnsiConsole.MarkupLine(
            $"[red][[Masked.Library]][/] [yellow italic]Building[/] [bold green underline]Application Commands[/] for [maroon underline]Discord Client[/] with signed user [red bold]{client.CurrentUser.Username}[/] [green italic]({client.CurrentUser.Id})[/]");

        await client.BulkOverwriteGlobalApplicationCommandsAsync(Commands.ToArray());

        AnsiConsole.MarkupLine(
            $"[red][[Masked.Library]][/] [yellow italic]SlashCommandHandler[/] [bold green underline]Loaded[/] for [maroon underline]Discord Client[/] with signed user [red bold]{client.CurrentUser.Username}[/] [green italic]({client.CurrentUser.Id})[/]");
        var closure = GetSlashCommandHandler();
        client.SlashCommandExecuted += async cmd => await closure?.Invoke(cmd)!;
    }
}