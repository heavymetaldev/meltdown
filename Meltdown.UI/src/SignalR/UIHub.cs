using Microsoft.AspNetCore.SignalR;

namespace Meltdown.UI.SignalR;

public interface IUIClient
{
    Task HandleLog(string path, string message);
    Task Progress(string path, string state, string status);
}

public class UIHub() : Hub<IUIClient>
{
}

