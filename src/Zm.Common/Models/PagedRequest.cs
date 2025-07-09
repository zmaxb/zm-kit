// ReSharper disable PropertyCanBeMadeInitOnly.Global

namespace Zm.Common.Models;

public class PagedRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public string? SortBy { get; set; }

    public bool Descending { get; set; }
    public Dictionary<string, object>? Filters { get; set; } = new();
}