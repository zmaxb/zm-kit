using Zm.Common.Abstractions;

namespace Zm.Common.Models;

public class PagedRequest<TFilter> : BasePagedRequest
{
    public TFilter? Filter { get; set; }
}