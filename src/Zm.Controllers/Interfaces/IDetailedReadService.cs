using System.Linq.Expressions;
using Zm.Common.Models;

namespace Zm.Controllers.Interfaces;

// ReSharper disable once TypeParameterCanBeVariant
public interface IDetailedReadService<TEntity, TDetailedDto, TKey>
{
    Task<TDetailedDto?> GetDetailedByIdAsync(TKey id);

    Task<(IEnumerable<TDetailedDto> Items, int TotalCount)> GetPagedDetailedAsync(PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null);
}