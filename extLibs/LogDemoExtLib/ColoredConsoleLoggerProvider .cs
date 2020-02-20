using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace LogDemoExtLib
{
    public class ColoredConsoleLoggerProvider : ILoggerProvider
    {
        private readonly IColoredConsoleLoggerConfiguration _config;
        private readonly ConcurrentDictionary<string, ColoredConsoleLogger> _loggers = new ConcurrentDictionary<string, ColoredConsoleLogger>();

        public ColoredConsoleLoggerProvider(IColoredConsoleLoggerConfiguration config)
        {
            _config = config;
        }

        public void Dispose()
        {
            _loggers.Clear();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return _loggers.GetOrAdd(categoryName, name => new ColoredConsoleLogger(name, _config));
        }
    }
}
