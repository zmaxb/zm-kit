using System.Linq.Expressions;
using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Extensions;

namespace Zm.Controllers.Services;

public abstract class BaseEntityService<TEntity, TKey>(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
    where TEntity : class
{
    protected readonly IMapper Mapper = mapper;
    protected readonly IGenericRepository<TEntity, TKey> Repository = repository;
    protected virtual int MaxPageSize => 200;

    protected async Task<(IEnumerable<TDto> Items, int TotalCount)> GetPagedInternalAsync<TDto>(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null,
        CancellationToken ct = default)
    {
        var safePaging = new PagingParameters(paging.Page, Math.Clamp(paging.PageSize, 1, MaxPageSize));
        var (entities, totalCount) = await Repository.GetPagedAsync(safePaging, filter, sort, ct);
        var mappedEntities = Mapper.SafeMapList<TEntity, TDto>(entities) ?? [];
        return (mappedEntities, totalCount);
    }
}