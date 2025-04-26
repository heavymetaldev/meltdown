using Microsoft.Extensions.Logging;

namespace HMDev.NodeUI;

public class SignalRLogger(SignalRQueue signalr, string categoryName) : ILogger
{
    public class Scope : IDisposable
    {
        public void Dispose() { }
    }

    public IDisposable BeginScope<TState>(TState state) where TState : notnull => new Scope();
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);

        signalr.Enqueue(categoryName, message);
    }
}

public class SignalRLoggerProvider(SignalRQueue signalr) : ILoggerProvider
{
    public ILogger CreateLogger(string categoryName) => new SignalRLogger(signalr, categoryName);
    public void Dispose() { }
}