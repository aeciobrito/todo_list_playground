using System;
using Microsoft.Extensions.Logging;

namespace TodoList.TestSupport.Spies;

public class SpyLogger<T> : ILogger<T>
{
    public List<LogEntry> Logs { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var message = formatter(state, exception);
        Logs.Add(new LogEntry(logLevel, message, exception));
    }
}

public record LogEntry(LogLevel Level, string Message, Exception? Exception);