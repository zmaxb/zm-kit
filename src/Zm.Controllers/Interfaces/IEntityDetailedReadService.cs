using System.Linq.Expressions;
using Zm.Common.Models;

namespace Zm.Controllers.Interfaces;

public interface IEntityDetailedReadService<TEntity, TDetailedDto, TKey>
{
    Task<TDetailedDto?> GetDetailedByIdAsync(TKey id, CancellationToken ct = default);

    Task<(IEnumerable<TDetailedDto> Items, int TotalCount)> GetPagedDetailedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default);
}