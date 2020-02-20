using System;
using Microsoft.Extensions.Logging;

namespace LogDemoExtLib
{
    public interface IColoredConsoleLoggerConfiguration
    {
        LogLevel LogLevel { get; set; }
        int EventId { get; set; }
        ConsoleColor Color { get; set; }
    }

    public class DefaultConsoleLoggerConfiguration : IColoredConsoleLoggerConfiguration
    {
        public const int DefaultEventId = 0;

        public LogLevel LogLevel { get; set; } = LogLevel.Warning;
        public int EventId { get; set; } = DefaultEventId;
        public ConsoleColor Color { get; set; } = ConsoleColor.Yellow;
    }
}
