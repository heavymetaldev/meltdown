using Microsoft.JavaScript.NodeApi;
using System.Text;

namespace Meltdown.UI.SignalR;


/// <summary>
/// Interface for command dispatcher.
/// The command dispatcher is used to register commands on the .NET side that can be invoked from the node.js side.
/// </summary>
public interface ICommandDispatcher
{
    /// <summary>
    /// Register a command on the .NET side that can be invoked from the node.js side.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <param name="action">The action to invoke when the command is invoked.</param>
    void On(string command, Action<string, string[]> action);
}

/// <summary>
/// Direct command dispatcher that uses the UICommandCallback (direct nodejs process communication)
/// </summary>
public class DirectCommandDispatcher(UICommandCallback callback) : ICommandDispatcher
{
    public void On(string command, Action<string, string[]> action)
    {
        callback.OnCommand += (cmd, path, args) =>
        {
            if (cmd == command)
            {
                action(path, args);
            }
        };
    }
}

/// <summary>
/// Command callback class that allows the node.js side to invoke commands on the .NET side.
/// usage in node.js:
/// commandEmitter.invokeCommand("doSomething", "path|whatever", ["arg1", "arg2"]);
/// </summary>
public class UICommandCallback
{
    public event Action<string, string, string[]>? OnCommand;
    
    /// <summary>
    /// Register the command on the node.js side.
    /// </summary>
    /// <param name="emitter">The command emitter (i.e. CommandEmitter exported from the node.js side)</param>
    public void Register(JSValue emitter)
        // the command emitter on js side emits one event called "invoke" with the command name, path, and an array of arguments
        => emitter.CallMethod("on", "invoke", JSValue.CreateFunction("invokeCommand", Invoke));

    private JSValue Invoke(JSCallbackArgs args)
    {
        var command = Encoding.UTF8.GetString(args[0].GetValueStringUtf8());
        var path = Encoding.UTF8.GetString(args[1].GetValueStringUtf8());

        var arrayItems = new string[args[2].GetArrayLength()];
        for (var i = 0; i < arrayItems.Length; i++)
        {
            arrayItems[i] = Encoding.UTF8.GetString(args[2][i].GetValueStringUtf8());
        }

        Invoke(command, path, arrayItems);

        return "OK";
    }

    private void Invoke(string command, string path, string[] args) => OnCommand?.Invoke(command, path, args);
}
