using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Reflection;

namespace Meltdown.UI;

public class NodeUI
{
    public record Options
    {
        public bool PatchConsole { get; set; } = true;
        public required Paths Paths { get; set; }
    }
    public record Paths
    {
        public required string LibNode { get; init; }
        public required string ClientApp { get; init; }

        public string BaseDir { get; internal init; } = string.Empty;
        public string WorkingDirectory => (Path.IsPathRooted(ClientApp) ? Path.GetDirectoryName(ClientApp) : Path.GetDirectoryName(Path.Combine(BaseDir, ClientApp))) ?? throw new Exception($"Could not determine working directory from ClientApp path: '{ClientApp}'");
        public string EntryPoint => Path.GetFileName(ClientApp);
    }

    public record DirectComm(IProgressReporter ProgressReporter, ICommandDispatcher CommandDispatcher);

    private static Paths GetPaths()
    {
        var baseDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var runtime = "win-x64";
        var libnodePath = Path.Combine(baseDir, $"runtimes/{runtime}/native", "libnode.dll");

        var clientApp = Path.Combine(baseDir, "CLI", "dist", "bundle.mjs");

        return new Paths { ClientApp = clientApp, LibNode = libnodePath, BaseDir = baseDir };
    }

    public static async Task<DirectComm> StartAsync(Func<Options, Options>? configure = null)
    {
        configure ??= o => o;
        var defaultOptions = new Options() { Paths = GetPaths() };
        var options = configure(defaultOptions);

        var paths = options.Paths;
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

            var progressReporter = new DirectProgressReporter(nodejsRuntime, "./" + paths.EntryPoint);
            if (options.PatchConsole)
            {
                Console.SetOut(new ProgressWriter(progressReporter));
            }

            var callback = new CommandCallback();
            await nodejsRuntime.RunAsync(async () =>
            {
                try
                {
                    var module = await nodejsRuntime.ImportAsync("./" + paths.EntryPoint, esModule: true);
                    var defaultExport = module.GetProperty("default");
                    var invoker = defaultExport.GetProperty(Exports.CommandEmitter);
                    if (invoker.IsNullOrUndefined())
                    {
                        throw new Exception($"missing export of '{Exports.CommandEmitter}' in {paths.EntryPoint}");
                    }
                    callback.Register(invoker);

                    defaultExport.CallMethod(Exports.MainMethod);
                }
                catch (Exception ex)
                {
                    throw;
                }
            });

            return new(progressReporter, new DirectCommandDispatcher(callback));
        }
        catch (Exception ex)
        {
            throw;
        }
    }
}

class Exports
{
    public const string MainMethod = "cli";
    public const string CommandEmitter = "commandEmitter";
    public const string ProgressEmitter = "progressEmitter";
}

