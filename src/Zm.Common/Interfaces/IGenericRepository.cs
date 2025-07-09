using Zm.Common.Models;

namespace Zm.Common.Interfaces;

public interface IGenericRepository<TEntity, in TKey> where TEntity : class
{
    string GetPrimaryKeyName();

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