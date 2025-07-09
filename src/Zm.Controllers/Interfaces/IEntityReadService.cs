using System.Linq.Expressions;
using Zm.Common.Models;

namespace Zm.Controllers.Interfaces;

// ReSharper disable once TypeParameterCanBeVariant
public interface IEntityReadService<TEntity, TKey, TReadDto>
{
    Task<TReadDto?> GetByIdAsync(TKey id, CancellationToken ct = default);

    [Obsolete]
    Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default);
    
    Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct = default);

    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);
}