using System.Text.Json.Serialization;

namespace Zm.Common.Models;

public readonly struct PaginationInfo<T>(int page, int pageSize, int totalCount)
{
    [JsonPropertyOrder(1)] public int Page { get; init; } = page;
    [JsonPropertyOrder(2)] public int PageSize { get; init; } = pageSize;
    [JsonPropertyOrder(3)] public int TotalCount { get; init; } = totalCount;

    [JsonPropertyOrder(4)]
    public int TotalPages { get; init; } = pageSize > 0
        ? (int)Math.Ceiling((double)totalCount / pageSize)
        : 0;

    [JsonPropertyOrder(5)] public IEnumerable<T> Items { get; init; } = [];
}