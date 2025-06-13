namespace Zm.Logging.Interfaces;

public interface ILoggerService<T>
{
    void LogInformation(string message, object? additionalData = null);
    void LogWarning(string message, object? additionalData = null);
    void LogError(string message, Exception? ex = null, object? additionalData = null);
    void LogDebug(string message, object? additionalData = null);
    void LogFatal(string message, Exception? ex = null, object? additionalData = null);
}
