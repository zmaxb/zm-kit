using System.Linq.Expressions;
using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Services;

public class EntityDetailedReadService<TEntity, TDetailedDto, TKey>
    : BaseEntityService<TEntity, TKey>, IEntityDetailedReadService<TEntity, TDetailedDto, TKey>
    where TEntity : class
{
    public EntityDetailedReadService(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
        : base(repository, mapper)
    {
    }

    public async Task<TDetailedDto?> GetDetailedByIdAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Repository.GetByIdAsync(id, ct);
        return entity is not null
            ? Mapper.SafeMap<TEntity, TDetailedDto>(entity)
            : default;
    }

    public async Task<(IEnumerable<TDetailedDto> Items, int TotalCount)> GetPagedDetailedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default)
    {
        return await GetPagedInternalAsync<TDetailedDto>(paging, filter, sort, ct);
    }
}