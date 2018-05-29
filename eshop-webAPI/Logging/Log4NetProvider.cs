using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;

public class Log4NetProvider : ILoggerProvider
{
    private static readonly NoopLogger _noopLogger = new NoopLogger();
    private readonly string _log4NetConfigFile;
    private readonly Func<string, bool> _sourceFilter;
    private readonly ConcurrentDictionary<string, Log4NetLogger> _loggers =
        new ConcurrentDictionary<string, Log4NetLogger>();

    public Log4NetProvider(string log4NetConfigFile, Func<string, bool> sourceFilter)
    {
        _log4NetConfigFile = log4NetConfigFile;
        _sourceFilter = sourceFilter;
    }

    public ILogger CreateLogger(string categoryName)
    {
        if (_sourceFilter != null && !_sourceFilter(categoryName))
            return _noopLogger;

        return _loggers.GetOrAdd(categoryName, CreateLoggerImplementation);
    }

    public void Dispose()
    {
        _loggers.Clear();
    }
    private Log4NetLogger CreateLoggerImplementation(string name)
    {
        return new Log4NetLogger(name, Parselog4NetConfigFile(_log4NetConfigFile));
    }

    private static XmlElement Parselog4NetConfigFile(string filename)
    {
        XmlDocument log4netConfig = new XmlDocument();
        log4netConfig.Load(File.OpenRead(filename));
        return log4netConfig["log4net"];
    }

    private class NoopLogger : ILogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }
    }
}