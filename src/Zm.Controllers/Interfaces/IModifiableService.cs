// ReSharper disable TypeParameterCanBeVariant
namespace Zm.Controllers.Interfaces;

public interface IModifiableService<TCreateDto, TUpdateDto, TKey>
{
    Task<TKey> CreateAsync(TCreateDto dto);
    Task<bool> UpdateAsync(TUpdateDto dto, TKey id);
    Task<int> DeleteAsync(TKey id);
}