using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Diagnostics;
using System.Reflection;
using static HMDev.NodeUI.SignalRQueue;

namespace HMDev.NodeUI;

public class NodeUI
{
    public record Paths
    {
        public required string LibNode { get; init; }
        public required string ClientApp { get; init; }

        public string BaseDir { get; internal init; } = string.Empty;
        public string WorkingDirectory => (Path.IsPathRooted(ClientApp) ? Path.GetDirectoryName(ClientApp) : Path.GetDirectoryName(Path.Combine(BaseDir, ClientApp))) ?? throw new Exception($"Could not determine working directory from ClientApp path: '{ClientApp}'");
        public string EntryPoint => Path.GetFileName(ClientApp);
    }

    public record DirectComm(IProgressReporter ProgressReporter, ICommandDispatcher CommandDispatcher);

    private class Exports
    {
        public const string MainMethod = "cli";
        public const string CommandEmitter = "commandEmitter";
    }

    private static Paths GetPaths()
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var runtime = "win-x64";
        var libnodePath = Path.Combine(baseDir, $"runtimes/{runtime}/native", "libnode.dll");

        var clientApp = Path.Combine(baseDir, "CLI", "node-ui.js");

        return new Paths { ClientApp = clientApp, LibNode = libnodePath, BaseDir = baseDir };
    }

    public static async Task<DirectComm> StartAsync(Func<Paths, Paths>? configure = null)
    {
        var paths = GetPaths();
        if (configure is not null)
        {
            paths = configure(paths);
        }

        var consoleHandler = new StringWriter();
        Console.SetOut(consoleHandler);


        var nodejsPlatform = new NodeEmbeddingPlatform(new NodeEmbeddingPlatformSettings()
        {
            LibNodePath = paths.LibNode,
        });

        try
        {
            var nodejsRuntime = nodejsPlatform.CreateThreadRuntime(paths.WorkingDirectory,
                new NodeEmbeddingRuntimeSettings
                {
                    MainScript = "globalThis.require = require('module').createRequire(process.execPath);\n",
                });

            var callback = new CommandCallback();
            await nodejsRuntime.RunAsync(async () =>
            {
                try
                {
                    var module = await nodejsRuntime.ImportAsync("./" + paths.EntryPoint, esModule: true);
                    var invoker = module.GetProperty(Exports.CommandEmitter);
                    callback.Register(invoker);

                    module.CallMethod(Exports.MainMethod);
                }
                catch (Exception ex)
                {
                    throw;
                }
            });

            return new(new DirectProgressReporter(nodejsRuntime), new DirectCommandDispatcher(callback));
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    public static DirectComm Start(Func<Paths, Paths>? configure = null)
        => StartAsync(configure).GetAwaiter().GetResult();
}
