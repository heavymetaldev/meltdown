using Meltdown.UI;
using Meltdown.UI.SignalR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddNodeUI(this IServiceCollection services)
    {
        services.AddSingleton<Func<IHubContext<UIHub, IUIClient>>>(ctx => {
            var context = ctx.GetRequiredService<IHubContext<UIHub, IUIClient>>();
            return () => context;
        });

        services.AddSingleton<ILoggerProvider, SignalRLoggerProvider>();

        services.AddSingleton<SignalRQueue>();
        services.AddHostedService<SignalRQueueProcessor>();
        //services.AddTransient<IProgressReporter, SignalRProgressReporter>();
    }
}
