//using Microsoft.AspNetCore.SignalR;

//namespace Meltdown.UI.SignalR;

//public class SignalRProgressReporter(IHubContext<UIHub, IUIClient> uiHub) : IProgressReporter
//{
//    public async Task ReportProgress(string fullPath, ProgressState state, string status, int progress)
//    {
//        await uiHub.Clients.All.HandleLog(fullPath, $"=== status: {state} ({status})");
//        await uiHub.Clients.All.Progress(fullPath, state.ToString().ToLower(), status);
//    }

//    public async Task Log(string fullPath, string message)
//    {
//        await uiHub.Clients.All.HandleLog(fullPath, message);
//    }

//    public Task Command(string fullPath, string command, string[] args)
//    {
//        throw new NotImplementedException();
//    }

//    public Task SetCommands(string fullPath, CommandDescription[] commands)
//    {
//        throw new NotImplementedException();
//    }
//}

