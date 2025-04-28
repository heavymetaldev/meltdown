// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting progress-list sample...");

var comm = await HMDev.NodeUI.NodeUI.StartAsync();
await comm.ProgressReporter.Command("root", "setVariant", ["simple"]);

int heartbeat = 0;
while (true)
{
    Console.WriteLine($"Heartbeat {heartbeat++}");
    await Task.Delay(1000);
}