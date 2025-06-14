using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Services;

public class CrudControllerService<TEntity, TCreateDto, TUpdateDto, TKey>
    : ControllerServiceBase<TEntity, TKey>, IModifiableService<TCreateDto, TUpdateDto, TKey>
    where TEntity : class
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public CrudControllerService(IGenericRepository<TEntity, TKey> repository,
        IMapper mapper) : base(repository, mapper)
    {
    }

    public async Task<TKey> CreateAsync(TCreateDto dto)
    {
        var entity = Mapper.SafeMap<TCreateDto, TEntity>(dto)
                     ?? throw new InvalidOperationException(
                         $"Mapping {typeof(TCreateDto).Name} to {typeof(TEntity).Name} failed.");

        await Repository.CreateAsync(entity);

        var keyPropertyName = Repository.GetPrimaryKeyName();
        var keyProperty = typeof(TEntity).GetProperty(keyPropertyName)
                          ?? throw new InvalidOperationException(
                              $"No key property '{keyPropertyName}' found on entity {typeof(TEntity).Name}.");

        var keyValue = keyProperty.GetValue(entity)
                       ?? throw new InvalidOperationException(
                           $"Entity key '{keyPropertyName}' is null after creation.");

        return (TKey)keyValue;
    }

    public async Task<bool> UpdateAsync(TUpdateDto dto, TKey id)
    {
        var entity = await Repository.GetByIdAsync(id);
        if (entity == null) return false;

        Mapper.Map(dto, entity);
        return await Repository.UpdateAsync(entity);
    }

    public async Task<int> DeleteAsync(TKey id)
    {
        return await SoftDeleteAsync(id);
    }

    private async Task<int> SoftDeleteAsync(TKey id)
    {
        if (!typeof(ISoftDeletableEntity).IsAssignableFrom(typeof(TEntity)))
            throw new NotSupportedException($"{typeof(TEntity).Name} does not support soft deletion.");

        if (Repository is not ISoftDeletable<TKey> softRepo)
            throw new InvalidOperationException(
                $"Repository does not implement ISoftDeletable for {typeof(TEntity).Name}");

        return await softRepo.SoftDeleteAsync(id);
    }
}