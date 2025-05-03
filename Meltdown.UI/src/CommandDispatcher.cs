using Microsoft.JavaScript.NodeApi;
using System.Text;

namespace Meltdown.UI;

public class CommandCallback
{
    public event Action<string, string, string[]>? OnCommand;
    public void Register(JSValue emitter)
    {
        emitter.CallMethod("on", "invoke", JSValue.CreateFunction("invokeCommand", Invoke));
    }

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

    private void Invoke(string command, string path, string[] args)
    {
        OnCommand?.Invoke(command, path, args);
    }
}

public interface ICommandDispatcher
{
    void On(string command, Action<string, string[]> action);
}

public class DirectCommandDispatcher(CommandCallback callback) : ICommandDispatcher
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