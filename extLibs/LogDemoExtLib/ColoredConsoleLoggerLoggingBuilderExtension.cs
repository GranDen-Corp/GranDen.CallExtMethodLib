using System;
using Microsoft.Extensions.Logging;

namespace LogDemoExtLib
{
    public static class ColoredConsoleLoggerLoggingBuilderExtension
    {
        /// <summary>
        /// Add Custom Color Console to logging pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to add logging provider to.</param>
        /// <param name="coloredConsoleLoggerConfiguration">log configuration object</param>
        /// <returns></returns>
        public static ILoggingBuilder AddColoredConsoleLog(this ILoggingBuilder builder, IColoredConsoleLoggerConfiguration coloredConsoleLoggerConfiguration)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.AddProvider(new ColoredConsoleLoggerProvider(coloredConsoleLoggerConfiguration));

            return builder;
        }

        /// <summary>
        /// Add Custom Color Console to logging pipeline.
        /// </summary>
        /// <param name="builder">The <see cref="T:Microsoft.Extensions.Logging.ILoggingBuilder" /> to add logging provider to.</param>
        /// <param name="configure">Custom log configuration Action</param>
        /// <returns></returns>
        public static ILoggingBuilder AddColoredConsoleLog(this ILoggingBuilder builder, Action<IColoredConsoleLoggerConfiguration> configure = null)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            var coloredConsoleLoggerConfiguration = new DefaultConsoleLoggerConfiguration();

            configure?.Invoke(coloredConsoleLoggerConfiguration);

            builder.AddProvider(new ColoredConsoleLoggerProvider(coloredConsoleLoggerConfiguration));

            return builder;
        }
    }
}
