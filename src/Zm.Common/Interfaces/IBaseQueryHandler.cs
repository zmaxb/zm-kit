using Zm.Common.Models;

namespace Zm.Common.Interfaces;

public interface IBaseQueryHandler<TDto, in TKey>
{
    Task<TDto?> GetByIdAsync(TKey key, CancellationToken ct = default);
    Task<bool> ExistsAsync(TKey key, CancellationToken ct = default);
    Task<IReadOnlyList<TDto>> GetPagedAsync(PagedRequest pagedRequest, CancellationToken ct = default);
}