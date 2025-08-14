using Microsoft.Extensions.Logging;

namespace Meltdown.UI.SignalR;

public class DirectUILogger(IProgressReporter progressReporter, string categoryName) : ILogger
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

        progressReporter.Log(categoryName, message);
    }
}

public class DirectUILoggerProvider(IProgressReporter progressReporter) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new DirectUILogger(progressReporter, categoryName);
    public void Dispose() { }
}