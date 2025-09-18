using System.Windows;
using CanvasTools.Windows.ViewModels;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace CanvasTools.Windows.Internal;

internal static class Logging
{
    /// <summary>
    /// Initialises the logging for the application.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the client to.</param>
    /// <param name="logPath">The path to log to.</param>
    /// <returns>A <see cref="ILogger"/> to log any start-up events.</returns>
    public static ILogger InitializeLogging(this IServiceCollection services, string logPath)
    {
        Guard.IsNotNull(services);
        Guard.IsNotNullOrWhiteSpace(logPath);

        var loggingContext = new InternalLoggingContext();
        services.AddSingleton(loggingContext);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Debug()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.ObservableCollection(
                loggingContext.Events,
                dispatcher: Application.Current.Dispatcher.Invoke,
                configure: options =>
                {
                    options.MaxStoredEvents = 2000;  // Maximum log events to store
                    options.MinimumLevel = LogEventLevel.Information;
                })
            .CreateLogger();

        services.AddSingleton<ILogger>(logger);
        return logger;
    }
}