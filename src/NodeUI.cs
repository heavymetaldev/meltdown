using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Reflection;
using static HMDev.NodeUI.SignalRQueue;

namespace HMDev.NodeUI;

public class NodeUI
{
    public record Paths
    {
        public required string LibNode { get; init; }
        public required string ClientApp { get; init; }
        public string WorkingDirectory => Path.GetDirectoryName(ClientApp) ?? throw new Exception($"Could not determine working directory from ClientApp path: '{ClientApp}'");
        public string EntryPoint => Path.GetFileName(ClientApp);

    }

    private static Paths GetPaths()
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var runtime = "win-x64";
        var libnodePath = Path.Combine(baseDir, $"runtimes/{runtime}/native", "libnode.dll");

        var clientApp = Path.Combine(baseDir, "CLI", "index.js");


        return new Paths { ClientApp = clientApp, LibNode = libnodePath };
    }

    public static async Task<(IProgressReporter, ICommandDispatcher)> StartAsync(Func<Paths, Paths>? configure = null)
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
                    var invoker = module.GetProperty("commandEmitter");
                    callback.Register(invoker);

                    module.CallMethod("cli");
                }
                catch (Exception ex)
                {
                    throw;
                }
            });

            return (new DirectProgressReporter(nodejsRuntime), new DirectCommandDispatcher(callback));
        } catch(Exception ex)
        {
            throw;
        }
    }

    public static (IProgressReporter, ICommandDispatcher) Start(Func<Paths, Paths>? configure = null) 
        => StartAsync(configure).GetAwaiter().GetResult();
}
