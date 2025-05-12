using Microsoft.AspNetCore.SignalR;
using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Text.Json;
using static Meltdown.UI.NodeUI;
using static Meltdown.UI.SignalRQueue;

namespace Meltdown.UI;

public enum ProgressState
{
    Unknown,
    Pending,
    Running,
    Done,
    Error,
}

public record CommandDescription(string key, string name, string description);

public interface IProgressReporter
{
    Task Log(string fullPath, string message);
    Task ReportProgress(string fullPath, ProgressState state, string status, int progress);
    Task Command(string fullPath, string command, string[] args);
    Task SetCommands(string fullPath, CommandDescription[] commands);
}

public class ProgressReporter(IHubContext<UIHub, IUIClient> uiHub) : IProgressReporter
{
    public async Task ReportProgress(string fullPath, ProgressState state, string status, int progress)
    {
        await uiHub.Clients.All.HandleLog(fullPath, $"=== status: {state} ({status})");
        await uiHub.Clients.All.Progress(fullPath, state.ToString().ToLower(), status);
    }

    public async Task Log(string fullPath, string message)
    {
        await uiHub.Clients.All.HandleLog(fullPath, message);
    }

    public Task Command(string fullPath, string command, string[] args)
    {
        throw new NotImplementedException();
    }

    public Task SetCommands(string fullPath, CommandDescription[] commands)
    {
        throw new NotImplementedException();
    }
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

