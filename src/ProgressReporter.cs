using Microsoft.AspNetCore.SignalR;
using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using static HMDev.NodeUI.NodeUI;
using static HMDev.NodeUI.SignalRQueue;

namespace HMDev.NodeUI;

public enum ProgressState
{
    Unknown,
    Pending,
    Running,
    Done,
    Error,
}

public interface IProgressReporter
{
    Task Log(string fullPath, string message);
    Task ReportProgress(string fullPath, ProgressState state, string status);
}

public class ProgressReporter(IHubContext<UIHub, IUIClient> uiHub) : IProgressReporter
{
    public async Task ReportProgress(string fullPath, ProgressState state, string status)
    {
        await uiHub.Clients.All.HandleLog(fullPath, $"=== status: {state} ({status})");
        await uiHub.Clients.All.Progress(fullPath, state.ToString().ToLower(), status);
    }

    public async Task Log(string fullPath, string message)
    {
        await uiHub.Clients.All.HandleLog(fullPath, message);
    }
}


class DirectProgressReporter(NodeEmbeddingThreadRuntime nodeRuntime) : IProgressReporter
{
    public async Task Log(string fullPath, string message)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync("./build/lib/utils/commands.js", esModule: true);
                var emitter = module.GetProperty("progressEmitter");
                emitter.CallMethod("log", fullPath, message);
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }

    public async Task ReportProgress(string fullPath, ProgressState state, string status)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync("./build/lib/utils/commands.js", esModule: true);
                var emitter = module.GetProperty("progressEmitter");
                emitter.CallMethod("update", fullPath, state.ToString().ToLower(), status);
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }
}

