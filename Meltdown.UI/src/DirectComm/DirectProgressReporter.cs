using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Text.Json;

namespace Meltdown.UI;

class DirectProgressReporter(NodeEmbeddingThreadRuntime nodeRuntime, string commandsModule = "./build/utils/commands.js") 
    : IProgressReporter, IUICommands
{
    CommandsModule commandsWrapper = new(nodeRuntime, commandsModule);
    public async Task Log(string fullPath, string message)
    {
        await commandsWrapper.WithProgressEmitter(async emitter =>
        {
            emitter.CallMethod("log", fullPath, message);
        });
    }

    public async Task ReportProgress(string fullPath, ProgressState state, string status, int progress)
    {
        await commandsWrapper.WithProgressEmitter((emitter) =>
        {
            var value = new JSObject(
            [
                new("state", state.ToString().ToLower()),
                    new("status", status),
                    new("progress", progress),
                ]);
            emitter.CallMethod("update", fullPath, value);
        });
    }

    public async Task InvokeUICommand(string fullPath, string command, string[] args)
    {
        await commandsWrapper.WithProgressEmitter(async emitter =>
        {
            emitter.CallMethod("command", fullPath, command, JsonSerializer.Serialize(args));
        });
    }

    public async Task ExposeServerCommands(string fullPath, CommandDescription[] commands)
    {
        await commandsWrapper.WithProgressEmitter(emitter =>
        {
            emitter.CallMethod("setCommands", fullPath, JsonSerializer.Serialize(commands));
        });
    }
}
