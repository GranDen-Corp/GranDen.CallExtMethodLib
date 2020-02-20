using System;
using System.Collections.Generic;
using GranDen.CallExtMethodLib;
using LogDemoExtLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CallLogDemoExtLibApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var demo = serviceProvider.GetService<MyClass>();

            Console.WriteLine("Run Demo:\r\n");

            demo.DemoRun();

            Console.WriteLine("\r\nPress enter to exit.");
            Console.ReadLine();
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging(loggingBuilder =>
            {
                //Add Trace logger
                loggingBuilder.AddColoredConsoleLog(config =>
                {
                    config.LogLevel = LogLevel.Trace;
                    config.Color = ConsoleColor.Green;
                });

                //Add Debug logger
                loggingBuilder.AddColoredConsoleLog(
                    new DefaultConsoleLoggerConfiguration { LogLevel = LogLevel.Debug, Color = ConsoleColor.Cyan });

                var helper = new ExtMethodInvoker("LogDemoExtLib");

                //Add Information logger's original code:
                //loggingBuilder.AddColoredConsoleLog(config =>
                //{
                //    config.LogLevel = LogLevel.Information;
                //    config.Color = ConsoleColor.Gray;
                //});
                var actionInputType = helper.ExtensionLibAssembly.GetType("LogDemoExtLib.IColoredConsoleLoggerConfiguration", true);
                var configAction =
                    CreateDelegateHelper.CreateAssignValueAction(actionInputType, "logConfig", new Dictionary<string, object>
                    {
                        ["LogLevel"] = LogLevel.Information,
                        ["Color"] = ConsoleColor.Gray
                    });
                helper.Invoke<ILoggingBuilder>(
                    new ExtMethodInfo
                    {
                        MethodName = "AddColoredConsoleLog",
                        ExtendedType = loggingBuilder.GetType().GetInterface("ILoggingBuilder")
                    },
                    loggingBuilder, configAction);

                //Add Warning logger's original code:
                //loggingBuilder.AddColoredConsoleLog(new DefaultConsoleLoggerConfiguration());
                var configObjectType =
                    helper.ExtensionLibAssembly.GetType("LogDemoExtLib.DefaultConsoleLoggerConfiguration", true);
                var configObj =  Activator.CreateInstance(configObjectType);
                helper.Invoke<ILoggingBuilder>(
                    new ExtMethodInfo
                    {
                        MethodName = "AddColoredConsoleLog",
                        ExtendedType = loggingBuilder.GetType().GetInterface("ILoggingBuilder")
                    },
                    loggingBuilder, configObj);
                
                //Add Error logger
                loggingBuilder.AddColoredConsoleLog(
                    new DefaultConsoleLoggerConfiguration { LogLevel = LogLevel.Error, Color = ConsoleColor.Red });

                //Add Critical logger
                loggingBuilder.AddColoredConsoleLog(config =>
                {
                    config.LogLevel = LogLevel.Critical;
                    config.Color = ConsoleColor.Magenta;
                });

                loggingBuilder.AddFilter<ColoredConsoleLoggerProvider>(typeof(MyClass).Namespace, LogLevel.Trace);
            });
            services.AddTransient<MyClass>();
        }

    }
}
