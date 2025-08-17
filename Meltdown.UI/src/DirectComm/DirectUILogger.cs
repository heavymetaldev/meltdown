using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Meltdown.UI.SignalR;

public class DirectUILogger(IProgressReporter progressReporter, DirectUILoggerOptions options, string categoryName) : ILogger
{
    public class Scope : IDisposable
    {
        public void Dispose() { }
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => new Scope();
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = formatter(state, exception);
        var path = options.SplitByCategory ? $"{options.Path}|{categoryName}" : options.Path;

        progressReporter.Log(path, message);
    }
}

public record DirectUILoggerOptions
{
       public string Path { get; set; } = "log|server";
       public bool SplitByCategory { get; set; } = false;
}

public class DirectUILoggerProvider(IProgressReporter progressReporter, IOptions<DirectUILoggerOptions> options) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new DirectUILogger(progressReporter, options.Value, categoryName);
    public void Dispose() { }
}