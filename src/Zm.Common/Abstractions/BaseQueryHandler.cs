using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Zm.Common.Helpers;
using Zm.Common.Models;

namespace Zm.Common.Abstractions;

public abstract class BaseQueryHandler<TDto, TEntity, TKey, TFilter>(DbContext ctx, IMapper mapper)
    where TEntity : class
{
    private readonly string _primaryKeyName = EfHelper.GetPrimaryKeyName<TEntity>(ctx);

    protected virtual IQueryable<TEntity> Query => ctx.Set<TEntity>().AsQueryable();

    protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string? search)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplyFilters(IQueryable<TEntity> query, TFilter? filter)
    {
        return query;
    }

    protected virtual IQueryable<TEntity> ApplySorting(IQueryable<TEntity> query, string? sortBy, bool descending)
    {
        return QueryableSortingHelper.Apply(query, sortBy, descending);
    }

    protected virtual IQueryable<TEntity> BeforeExecute(IQueryable<TEntity> query)
    {
        return query;
    }

    public virtual async Task<TDto?> GetByIdAsync(TKey id, CancellationToken ct = default)
    {
        var entity = await Query.FirstOrDefaultAsync(
            x => EF.Property<TKey>(x, _primaryKeyName)!.Equals(id),
            ct
        );

        return entity is null ? default : mapper.Map<TDto>(entity);
    }

    public virtual async Task<bool> ExistsAsync(TKey id, CancellationToken ct = default)
    {
        return await Query.AnyAsync(
            x => EF.Property<TKey>(x, _primaryKeyName)!.Equals(id),
            ct
        );
    }

    public virtual async Task<PaginationInfo<TDto>> GetPagedAsync(PagedRequest<TFilter> request,
        CancellationToken ct = default)
    {
        var query = Query;

        query = ApplySearch(query, request.Search);
        query = ApplyFilters(query, request.Filter);
        query = ApplySorting(query, request.SortBy, request.Descending);
        query = BeforeExecute(query);

        var total = await query.CountAsync(ct);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var dtos = mapper.Map<IReadOnlyList<TDto>>(items);

        return new PaginationInfo<TDto>(request.Page, request.PageSize, total, dtos);
    }
}