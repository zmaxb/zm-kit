using Zm.Common.Abstractions;

namespace Zm.Common.Models;

public class PagedRequest : BasePagedRequest
{
    public Dictionary<string, object>? Filters { get; set; } = new();
}