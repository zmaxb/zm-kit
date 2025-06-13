namespace D3us.Common.Interfaces.Repositories;

// ReSharper disable once TypeParameterCanBeVariant
public interface ISoftDeletable<TKey>
{
    Task<int> SoftDeleteAsync(TKey id);
}