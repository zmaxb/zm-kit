using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;

namespace Zm.Logging.Static;

public static class Logger
{
    private static ILogger Instance { get; set; } = Log.Logger;

    public static void ConfigureLogging(string serviceName, IConfiguration configuration)
    {
        var seqUrl = configuration["Logging:SeqUrl"];

        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.WithProperty("Service", serviceName)
            .Enrich.FromLogContext();

        if (!string.IsNullOrWhiteSpace(seqUrl)) loggerConfig = loggerConfig.WriteTo.Seq(seqUrl);

        Instance = loggerConfig.CreateLogger();
        Log.Logger = Instance;
    }

    public static void LogInformation(
        string message,
        object? additionalData = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        Write(LogEventLevel.Information, message, null, additionalData, sourceFilePath);
    }

    public static void LogWarning(
        string message,
        object? additionalData = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        Write(LogEventLevel.Warning, message, null, additionalData, sourceFilePath);
    }

    public static void LogError(
        string message,
        Exception? ex = null,
        object? additionalData = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        Write(LogEventLevel.Error, message, ex, additionalData, sourceFilePath);
    }

    public static void LogDebug(
        string message,
        object? additionalData = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        Write(LogEventLevel.Debug, message, null, additionalData, sourceFilePath);
    }

    public static void LogFatal(
        string message,
        Exception? ex = null,
        object? additionalData = null,
        [CallerFilePath] string sourceFilePath = "")
    {
        Write(LogEventLevel.Fatal, message, ex, additionalData, sourceFilePath);
    }

    private static void Write(
        LogEventLevel level,
        string message,
        Exception? ex,
        object? additionalData,
        string sourceFilePath)
    {
        if (!Instance.IsEnabled(level)) return;

        var sourceContext = Path.GetFileNameWithoutExtension(sourceFilePath);

        var log = Instance.ForContext("SourceContext", sourceContext);

        if (additionalData != null) log = log.ForContext("AdditionalData", additionalData, true);

        log.Write(level, ex, message);
    }
}