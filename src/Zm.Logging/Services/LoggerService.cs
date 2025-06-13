using Microsoft.Extensions.Logging;
using Zm.Logging.Interfaces;

namespace Zm.Logging.Services;

public class LoggerService<T>(ILogger<T> logger) : ILoggerService<T>
{
    public void LogInformation(string message, object? additionalData = null) =>
        logger.LogInformation("{Message} {@AdditionalData}", message, additionalData);

    public void LogWarning(string message, object? additionalData = null) =>
        logger.LogWarning("{Message} {@AdditionalData}", message, additionalData);

    public void LogError(string message, Exception? ex = null, object? additionalData = null) =>
        logger.LogError(ex, "{Message} {@AdditionalData}", message, additionalData);

    public void LogDebug(string message, object? additionalData = null) =>
        logger.LogDebug("{Message} {@AdditionalData}", message, additionalData);

    public void LogFatal(string message, Exception? ex = null, object? additionalData = null) =>
        logger.LogCritical(ex, "{Message} {@AdditionalData}", message, additionalData);
}