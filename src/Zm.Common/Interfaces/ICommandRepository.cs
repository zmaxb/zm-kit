namespace Zm.Common.Interfaces;

public interface 
    ICommandRepository<in TEntity, TKey> where TEntity : class
{
    Task<TKey> CreateAsync(TEntity entity, CancellationToken ct = default);
    Task<bool> UpdateAsync(TEntity entity, CancellationToken ct = default);
    Task<int> DeleteAsync(TKey key, CancellationToken ct = default);
}