using System.Linq.Expressions;
using D3us.Common.Models;

namespace Zm.Controllers.Interfaces;

// ReSharper disable once TypeParameterCanBeVariant
public interface IReadOnlyService<TEntity, TKey, TReadDto>
{
    Task<TReadDto?> GetByIdAsync(TKey id);

    Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null);

    Task<bool> ExistsAsync(TKey id);
}