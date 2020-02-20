using System;
using System.Collections.Generic;
using System.Text;
using GranDen.CallExtMethodLib;
using LogDemoExtLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LibTestProject
{
    public class InvokeOverloadExtMethodTest
    {
        [Fact]
        public void InvokeLogAddBuilderMethodWithOptionObjectTest()
        {
            //Arrange
            const LogLevel defaultLogLevel = LogLevel.Information;
            const int defaultEventId = 1;

            var mockOption = new Mock<IColoredConsoleLoggerConfiguration>();
            mockOption.Setup(m => m.LogLevel).Returns(defaultLogLevel);
            mockOption.Setup(m => m.EventId).Returns(defaultEventId);
            mockOption.Setup(m => m.Color).Returns(ConsoleColor.Cyan);

            var serviceCollection = new ServiceCollection();

            //Act
            serviceCollection.AddLogging(loggingBuilder =>
            {
                var helper = new ExtMethodInvoker("LogDemoExtLib");
                helper.Invoke<ILoggingBuilder>(
                    new ExtMethodInfo
                    {
                        MethodName = "AddColoredConsoleLog",
                        ExtendedType = loggingBuilder.GetType().GetInterface("ILoggingBuilder")
                    },
                    loggingBuilder, mockOption.Object);
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerProvider = (ILoggerProvider)serviceProvider.GetService(typeof(ILoggerProvider));

            //Assert
            Assert.NotNull(loggerProvider);
            
            var logger = loggerProvider.CreateLogger($"{nameof(InvokeLogAddBuilderMethodWithOptionObjectTest)}");
            Assert.NotNull(logger);
            Assert.True(logger.IsEnabled(defaultLogLevel));
            logger.LogInformation(defaultEventId, "test logging");
            
            mockOption.VerifyGet(m => m.LogLevel);
            mockOption.VerifyGet(m => m.EventId);
            mockOption.VerifyGet(m => m.Color);
        }

        [Fact]
        public void InvokeLogAddBuilderMethodWithActionDelegate()
        {
            //Arrange
            const LogLevel defaultLogLevel = LogLevel.Information;
            const int defaultEventId = 1;
            const ConsoleColor defaultConsoleColor = ConsoleColor.Cyan;
            var serviceCollection = new ServiceCollection();

            //Act
            serviceCollection.AddLogging(loggingBuilder =>
            {
                var helper = new ExtMethodInvoker("LogDemoExtLib");
                var actionInputType = helper.ExtensionLibAssembly.GetType("LogDemoExtLib.IColoredConsoleLoggerConfiguration", true);
                var configAction =
                    CreateDelegateHelper.CreateAssignValueAction(actionInputType, "logConfig", new Dictionary<string, object>
                    {
                        ["LogLevel"] = defaultLogLevel,
                        ["EventId"] = defaultEventId,
                        ["Color"] = defaultConsoleColor
                    });

                helper.Invoke<ILoggingBuilder>(
                    new ExtMethodInfo
                    {
                        MethodName = "AddColoredConsoleLog",
                        ExtendedType = loggingBuilder.GetType().GetInterface("ILoggingBuilder")
                    },
                    loggingBuilder, configAction);
            });

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var loggerProvider = (ILoggerProvider)serviceProvider.GetService(typeof(ILoggerProvider));

            //Assert
            Assert.NotNull(loggerProvider);
            var logger = loggerProvider.CreateLogger($"{nameof(InvokeLogAddBuilderMethodWithOptionObjectTest)}");
            Assert.NotNull(logger);
            Assert.True(logger.IsEnabled(defaultLogLevel));
            var consoleLogger = logger as ColoredConsoleLogger;
            Assert.NotNull(consoleLogger);
            Assert.Equal(defaultEventId, consoleLogger.LoggerConfiguration.EventId);
            Assert.Equal(defaultConsoleColor, consoleLogger.LoggerConfiguration.Color);
        }
    }
}
