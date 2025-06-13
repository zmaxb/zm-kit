using System.Text.Json.Serialization;

namespace Zm.Common.Models;

public readonly struct ApiError(string id, string message)
{
    [JsonPropertyOrder(1)] public string Id { get; init; } = id;
    [JsonPropertyOrder(2)] public string Message { get; init; } = message;

    public static List<ApiError> FromStrings(IEnumerable<string> errors)
    {
        return errors.Select((message, index) => new ApiError((index + 1).ToString(), message)).ToList();
    }
}