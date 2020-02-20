using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CallLogDemoExtLibApp
{
    class MyClass
    {
        private readonly ILogger _logger;

        public MyClass(ILogger<MyClass> logger)
        {
            _logger = logger;
        }

        public void DemoRun()
        {
            _logger.LogTrace("This is trace log");
            _logger.LogDebug("This is debug log");
            _logger.LogInformation("This is information log");
            _logger.LogWarning("This is warning log");
            _logger.LogError("This is error log");
            _logger.LogCritical("This is critical log");
        }

    }
}
