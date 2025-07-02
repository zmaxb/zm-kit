namespace Zm.Controllers.Interfaces;

public interface IEntityCrudService<TCreateDto, TUpdateDto, TKey>
{
    Task<TKey> CreateAsync(TCreateDto dto, CancellationToken ct = default);

    Task<bool> UpdateAsync(TUpdateDto dto, TKey id, CancellationToken ct = default);

    Task<int> DeleteAsync(TKey id, CancellationToken ct = default);
}