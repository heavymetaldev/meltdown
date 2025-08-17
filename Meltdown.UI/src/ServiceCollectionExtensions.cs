using Meltdown.UI;
using Meltdown.UI.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddNodeUI(this IServiceCollection services)
    {
        //AddSignalRLogger(services);
        AddDirectLogger(services);

        //services.AddTransient<IProgressReporter, SignalRProgressReporter>();
    }

    private static void AddDirectLogger(IServiceCollection services, Action<DirectUILoggerOptions>? configure = null)
    {
        configure ??= options => { };
        services.Configure<DirectUILoggerOptions>(configure);
        services.AddSingleton<ILoggerProvider, DirectUILoggerProvider>();
    }

    private static void AddSignalRLogger(IServiceCollection services)
    {
        services.AddSingleton<Func<IHubContext<UIHub, IUIClient>>>(ctx =>
        {
            var context = ctx.GetRequiredService<IHubContext<UIHub, IUIClient>>();
            return () => context;
        });

        services.AddSingleton<SignalRQueue>();
        services.AddHostedService<SignalRQueueProcessor>();
        services.AddSingleton<ILoggerProvider, SignalRLoggerProvider>();
    }
}
