// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting progress-list sample...");

var comm = await Meltdown.UI.NodeUI.StartAsync();
await comm.ProgressReporter.Command("root", "setVariant", ["simple"]);

IEnumerable<string> GatherData()
{
    for (int i = 0; i < 100; i++)
    {
        Thread.Sleep(100);
        Console.WriteLine($"progress: {i}%");
        yield return $"Item {i}";
    }
}

// you can simply use console.writeline
Console.WriteLine("Gathering data...");
var data = GatherData().ToList();
Console.WriteLine("Got it!");