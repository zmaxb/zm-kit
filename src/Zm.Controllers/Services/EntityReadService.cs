using System.Linq.Expressions;
using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Abstractions;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Services;

public class EntityReadService<TEntity, TKey, TReadDto>
    : BaseEntityService<TEntity, TKey>, IEntityReadService<TEntity, TKey, TReadDto>
    where TEntity : class
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public EntityReadService(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
        : base(repository, mapper)
    {
    }

    public async Task<TReadDto?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        return entity is not null
            ? Mapper.SafeMap<TEntity, TReadDto>(entity)
            : default;
    }

    [Obsolete("Obsolete")]
    public async Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default)
    {
        return await GetPagedInternalAsync<TReadDto>(paging, filter, sort, ct);
    }
    
    public async Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(
        PagedRequest request,
        CancellationToken ct = default)
    {
        return await GetPagedInternalAsync<TReadDto>(request, ct);
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default)
    {
        return await Repository.ExistsAsync(id, ct);
    }
}