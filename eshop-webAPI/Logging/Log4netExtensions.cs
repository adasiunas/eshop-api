using Microsoft.Extensions.Logging;
using System;

public static class Log4netExtensions
{
    public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, string log4NetConfigFile, Func<string, bool> filter = null)
    {
        factory.AddProvider(new Log4NetProvider(log4NetConfigFile, filter));
        return factory;
    }

    public static ILoggerFactory AddLog4Net(this ILoggerFactory factory, Func<string, bool> filter = null)
    {
        factory.AddProvider(new Log4NetProvider("log4net.config", filter));
        return factory;
    }
}