using Meltdown.UI.SignalR;
using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Meltdown.UI;

public enum ProgressState
{
    Unknown,
    Pending,
    Ready,
    Running,
    Done,
    Error,
}

public record CommandDescription(string key, string name, string description)
{
    [JsonIgnore]
    public Func<Task>? handler { get; init; }
}


/// <summary>
/// The progress reporter is used to report progress to the node.js side.
/// </summary>
public interface IProgressReporter
{
    /// <summary>
    /// Log a message to the node.js side.
    /// </summary>
    Task Log(string fullPath, string message);

    /// <summary>
    /// Report progress to the node.js side.
    /// </summary>
    Task ReportProgress(string fullPath, ProgressState state, string status, int progress);

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

class DirectProgressReporter(NodeEmbeddingThreadRuntime nodeRuntime, string commandsModule = "./build/utils/commands.js") : IProgressReporter
{
    public async Task Log(string fullPath, string message)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync(commandsModule, esModule: true);
                var emitter = module.GetProperty("default").GetProperty(Exports.ProgressEmitter);
                emitter.CallMethod("log", fullPath, message);
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }

    public async Task ReportProgress(string fullPath, ProgressState state, string status, int progress)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync(commandsModule, esModule: true);
                var emitter = module.GetProperty("default").GetProperty(Exports.ProgressEmitter);
                var value = new JSObject(
                [
                    new("state", state.ToString().ToLower()),
                    new("status", status),
                    new("progress", progress),
                ]);
                emitter.CallMethod("update", fullPath, value);
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }

    public async Task InvokeUICommand(string fullPath, string command, string[] args)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync(commandsModule, esModule: true);
                var emitter = module.GetProperty("default").GetProperty(Exports.ProgressEmitter);
                emitter.CallMethod("command", fullPath, command, JsonSerializer.Serialize(args));
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }

    public async Task ExposeServerCommands(string fullPath, CommandDescription[] commands)
    {
       await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync(commandsModule, esModule: true);
                var emitter = module.GetProperty("default").GetProperty(Exports.ProgressEmitter);
                emitter.CallMethod("setCommands", fullPath, JsonSerializer.Serialize(commands));
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }
}

public static class ProgressReporterExtensions
{
    public static async Task ExposeServerCommands(this IProgressReporter progressReporter, string fullPath, ICommandDispatcher commandDispatcher, CommandDescription[] commands)
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

