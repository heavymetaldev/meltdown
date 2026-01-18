using Meltdown.UI.SignalR;
using System.Text.Json.Serialization;

namespace Meltdown.UI;

public record CommandDescription(string key, string name, string description)
{
    [JsonIgnore]
    public Func<Task>? handler { get; init; }
}

public interface IUICommands
{

    /// <summary>
    /// Execute a command on the node.js side.
    /// </summary>
    /// <param name="fullPath">The full path of the node.js module to execute the command on.</param>
    /// <param name="command">The command to execute.</param>
    /// <param name="args">The arguments to pass to the command.</param>
    Task InvokeUICommand(string fullPath, string command, string[] args);

    /// <summary>
    /// Set the commands that are available to the node.js side.
    /// </summary>
    /// <param name="fullPath">The full path of the node.js module to set the commands on.</param>
    /// <param name="commands">The commands to set.</param>
    Task ExposeServerCommands(string fullPath, CommandDescription[] commands);
}

public static class UICommandsExtensions
{
    public static async Task ExposeServerCommands(this IUICommands progressReporter, string fullPath, ICommandDispatcher commandDispatcher, CommandDescription[] commands)
    {
        commandDispatcher.RegisterCommands(fullPath, commands);
        await progressReporter.ExposeServerCommands(fullPath, commands);
    }

    private static void RegisterCommands(this ICommandDispatcher commandDispatcher, string fullPath, CommandDescription[] commands)
    {
        foreach (var command in commands)
        {
            if (command.handler != null)
            {
                commandDispatcher.On(command.name, (path, @args) =>
                {
                    if (path == fullPath)
                    {
                        _ = Task.Run(async () =>
                        {
                            await command.handler();
                        });
                    }
                });
            }
        }
    }
}
