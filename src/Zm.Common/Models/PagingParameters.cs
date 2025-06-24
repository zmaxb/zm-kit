// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace Zm.Common.Models;

public struct PagingParameters(int page, int pageSize)
{
    public int Page { get; set; } = page < 1 ? 1 : page;
    public int PageSize { get; set; } = pageSize < 1 ? 10 : pageSize;
}