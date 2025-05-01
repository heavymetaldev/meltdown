// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting progress-list sample...");

var comm = await HMDev.NodeUI.NodeUI.StartAsync(p => p with
{
    ClientApp = Path.Combine(p.BaseDir, "CLI", "dist/bundle.js"),
});
await comm.ProgressReporter.Command("root", "setVariant", ["simple"]);

int heartbeat = 0;
while (true)
{
    // console will be redirected to NodeUI
    Console.WriteLine($"Heartbeat {heartbeat++}");
    // but you can also use the progress reporter directly
    await comm.ProgressReporter.Log("root", $"Heartbeat {heartbeat}");
    await Task.Delay(1000);
}