using System.Linq.Expressions;
using Zm.Common.Models;

namespace Zm.Common.Interfaces;

// ReSharper disable once TypeParameterCanBeVariant
public interface IGenericRepository<TEntity, TKey> where TEntity : class
{
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null);

    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetPagedAsync(PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null
    );

    Task<TEntity?> GetByIdAsync(TKey id);

    Task<bool> CreateAsync(TEntity entity);
    Task<bool> UpdateAsync(TEntity entity);
    Task<int> DeleteAsync(TKey id);

    Task<bool> ExistsAsync(TKey id);
    string GetPrimaryKeyName();

    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}