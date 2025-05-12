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

public interface IProgressReporter
{
    Task Log(string fullPath, string message);
    Task ReportProgress(string fullPath, ProgressState state, string status, int progress);
    Task Command(string fullPath, string command, string[] args);
    Task SetCommands(string fullPath, CommandDescription[] commands);
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

    public async Task Command(string fullPath, string command, string[] args)
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


    public async Task SetCommands(string fullPath, CommandDescription[] commands)
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
    public static async Task SetCommands(this IProgressReporter progressReporter, string fullPath, ICommandDispatcher commandDispatcher, CommandDescription[] commands)
    {
        foreach (var command in commands)
        {
            if (command.handler != null)
            {
                commandDispatcher.On(command.name, (path, @args) =>
                {
                    if (path == fullPath)
                    {
                        _ = Task.Run(async () => {
                            await command.handler();
                        });
                    }
                });
            }
        }

        await progressReporter.SetCommands(fullPath, commands);
    }
}

