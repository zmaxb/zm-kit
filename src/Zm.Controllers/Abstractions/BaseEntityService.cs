using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Extensions;

namespace Zm.Controllers.Abstractions;

public abstract class BaseEntityService<TEntity, TKey>(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
    where TEntity : class
{
    protected readonly IMapper Mapper = mapper;
    protected readonly IGenericRepository<TEntity, TKey> Repository = repository;
    protected virtual int MaxPageSize => 200;

    protected async Task<(IEnumerable<TDto> Items, int TotalCount)> GetPagedInternalAsync<TDto>(
        PagedRequest request,
        CancellationToken ct = default)
    {
        var (entities, totalCount) = await Repository.GetPagedAsync(request, ct);
        var mappedEntities = Mapper.SafeMapList<TEntity, TDto>(entities) ?? [];
        return (mappedEntities, totalCount);
    }
}