namespace Zm.Common.Interfaces;

// ReSharper disable once TypeParameterCanBeVariant
public interface ISoftDeletable<TKey>
{
    Task<int> SoftDeleteAsync(TKey id);
}