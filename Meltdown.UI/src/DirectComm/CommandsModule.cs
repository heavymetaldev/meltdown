using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;

namespace Meltdown.UI;

public class CommandsModule(NodeEmbeddingThreadRuntime nodeRuntime, string commandsModule = "./build/utils/commands.js")
{
    public async Task WithProgressEmitter(Action<JSValue> call)
    {
        await nodeRuntime.RunAsync(async () =>
        {
            try
            {
                var module = await nodeRuntime.ImportAsync(commandsModule, esModule: true);
                var emitter = module.GetProperty("default").GetProperty(Exports.ProgressEmitter);
                call(emitter);
            }
            catch (Exception ex)
            {
                throw;
            }
        });
    }
}