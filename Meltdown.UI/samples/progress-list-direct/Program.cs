// See https://aka.ms/new-console-template for more information
using Meltdown.UI;

Console.WriteLine("Starting progress-list sample...");

var dataRunning = false;

var comm = await NodeUI.StartAsync();
var progress = comm.ProgressReporter;
// await comm.ProgressReporter.Command("root", "setVariant", ["master-detail"]); // master-detail is the default already
await comm.ProgressReporter.SetCommands("data", comm.CommandDispatcher, [
    new CommandDescription("s", "start", "start gathering data") {
        handler = async () => {
            if (dataRunning)
            {
                Console.WriteLine("Already gathering data...");
                return;
            }
            dataRunning = true;
            // you can simply use console.writeline
            Console.WriteLine("Gathering data...");
            var data = GatherData().ToList();
            Console.WriteLine("Got it!");
        }
    }
]);

IEnumerable<string> GatherData()
{
    int total = 100;
    for (int i = 0; i < total; i++)
    {
        Thread.Sleep(1000);
        // but it's better to use progress reporter
        progress.ReportProgress("data", Meltdown.UI.ProgressState.Running, $"Item {i} of {total}", (int)(i / (double)total * 100));
        progress.Log($"data|{i}", $"Got Item {i} of {total}");
        yield return $"Item {i}";
    }

    progress.ReportProgress("data", Meltdown.UI.ProgressState.Done, "DONE", 100);
}
