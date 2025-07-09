using System.Linq.Expressions;
using Zm.Common.Models;

namespace Zm.Common.Interfaces;

public interface IGenericRepository<TEntity, in TKey> where TEntity : class
{
    string GetPrimaryKeyName();

    [Obsolete]
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default);

    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(PagedRequest request,
        CancellationToken ct = default);

    Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);

    Task<bool> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task<int> DeleteAsync(TKey id, CancellationToken ct = default);

    Task<bool> ExistsAsync(TKey id, CancellationToken ct = default);

    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitTransactionAsync(CancellationToken ct = default);
    Task RollbackTransactionAsync(CancellationToken ct = default);
}