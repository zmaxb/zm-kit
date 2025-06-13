using System.Text.Json.Serialization;

namespace Zm.Common.Models;

public class ApiResponse<T>()
{
    [JsonPropertyOrder(1)] public bool Success { get; init; }
    [JsonPropertyOrder(2)] public string Message { get; init; } = string.Empty;
    [JsonPropertyOrder(3)] public T? Data { get; init; }
    [JsonPropertyOrder(4)] public List<ApiError> Errors { get; init; } = [];

    [JsonPropertyOrder(5)] public long ServerTime { get; init; } = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    [JsonPropertyOrder(6)] public string TraceId { get; init; } = string.Empty;

    public ApiResponse(
        bool success,
        string message,
        T? data = default,
        List<ApiError>? errors = null) : this()
    {
        Success = success;
        Message = message;
        Data = data;
        Errors = errors ?? [];
    }

    public static ApiResponse<T> Ok(T data, string message = "OK")
        => new(true, message, data);

    public static ApiResponse<T> Fail(string message)
        => new(false, message);

    public static ApiResponse<T> Fail(List<ApiError> errorMessages, string message = "Error processing the request")
        => new(false, message, default, errorMessages);

}