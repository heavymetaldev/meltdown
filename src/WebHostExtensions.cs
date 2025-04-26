using HMDev.NodeUI;
using Microsoft.AspNetCore.Routing;

namespace Microsoft.AspNetCore.Builder;

public static class WebApplicationExtensions
{
    public static void MapNodeUI(this IEndpointRouteBuilder app)
    {
        app.MapHub<UIHub>("/signalr/ui");
    }
}
