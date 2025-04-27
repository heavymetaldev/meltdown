// See https://aka.ms/new-console-template for more information
Console.WriteLine("Starting progress-list sample...");

var comm = await HMDev.NodeUI.NodeUI.StartAsync(c =>
{
    var paths = c with
    {
        ClientApp = "CLI/index.js"
    };

    Console.WriteLine($"CWD: {Environment.CurrentDirectory}");
    Console.WriteLine(paths);

    return paths;
});