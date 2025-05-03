// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting progress-list sample...");

var comm = await Meltdown.UI.NodeUI.StartAsync();
await comm.ProgressReporter.Command("root", "setVariant", ["simple"]);

IEnumerable<string> GatherData()
{
    for (int i = 0; i < 100; i++)
    {
        Thread.Sleep(100);
        yield return $"Item {i}";
    }
}

Console.WriteLine("Gathering data...");
var data = GatherData();
Console.WriteLine("Got it!");