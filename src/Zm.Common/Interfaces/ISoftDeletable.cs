namespace Zm.Common.Interfaces;

public interface ISoftDeletable<TKey>
{
    Task<int> SoftDeleteAsync(TKey id, CancellationToken ct = default);
}