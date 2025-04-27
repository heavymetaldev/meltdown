// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

await HMDev.NodeUI.NodeUI.StartAsync(c => c with {
    ClientApp = "CLI/index.js"
});