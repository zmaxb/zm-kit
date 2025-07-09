namespace Zm.Common.Interfaces;

public interface ISoftDeletable<in TKey>
{
    Task<int> SoftDeleteAsync(TKey id, CancellationToken ct = default);
    Task<int> RestoreAsync(TKey id, CancellationToken ct = default);
}