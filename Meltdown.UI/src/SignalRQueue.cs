using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;

namespace Meltdown.UI;

public class SignalRQueue
{
    public record Message(string categoryName, string message);
    private readonly Queue<Message> messages = new();
    public void Enqueue(string categoryName, string message) => messages.Enqueue(new Message(categoryName, message));

    public Message Dequeue() => messages.Dequeue();

    public bool IsEmpty() => messages.Count == 0;
}

public class SignalRQueueProcessor(SignalRQueue queue, Func<IHubContext<UIHub, IUIClient>?> contextProvider) : IHostedService
{
    private CancellationTokenSource CancellationTokenSource = new();
    private Task? runner;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        var runToken = CancellationTokenSource.Token;
        runner = Task.Run(async () => await Run(runToken));

        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        CancellationTokenSource.Cancel();

        if (runner is not null)
        {
            await runner;
        }
    }

    private async Task Run(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {

                var context = contextProvider();
                if (context is not null)
                {
                    while (!queue.IsEmpty())
                    {
                        var (category, message) = queue.Dequeue();
                        try
                        {
                            await context.Clients.All.HandleLog("server", message);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await Task.Delay(100);
        }
    }
}
