using System;
using Microsoft.Extensions.Logging;

namespace LogDemoExtLib
{
    public class ColoredConsoleLogger : ILogger
    {
        private readonly string _name;
        public IColoredConsoleLoggerConfiguration LoggerConfiguration { get; }

        public ColoredConsoleLogger(string name, IColoredConsoleLoggerConfiguration loggerConfiguration)
        {
            _name = name;
            LoggerConfiguration = loggerConfiguration;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) { return; }

            if (LoggerConfiguration.EventId == DefaultConsoleLoggerConfiguration.DefaultEventId || LoggerConfiguration.EventId == eventId.Id)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = LoggerConfiguration.Color;
                Console.WriteLine($"[{logLevel}] - {eventId.Id} - {_name} - {formatter(state, exception)}");
                Console.ForegroundColor = color;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel == LoggerConfiguration.LogLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }
    }
}
