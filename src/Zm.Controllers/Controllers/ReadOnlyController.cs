using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zm.Common.Models;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Controllers;

[ApiController]
public abstract class ReadOnlyController<TEntity, TKey, TReadDto>(
    IEntityReadService<TEntity, TKey, TReadDto> service)
    : ControllerBase
{
    private Func<string, Expression<Func<TEntity, bool>>>? _searchBuilder;
    private Func<string?, bool, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? _sortBuilder;
    public IEntityReadService<TEntity, TKey, TReadDto> EntityReadService { get; } = service;

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve item by ID")]
    public virtual async Task<ActionResult<ApiResponse<TReadDto>>> GetById(TKey id)
    {
        return await EntityReadService.GetByIdAsync(id) is { } item
            ? Ok(ApiResponse<TReadDto>.Ok(item))
            : NotFound(ApiResponse<string>.Fail("Not found"));
    }

    [HttpGet("{id}/exists")]
    [SwaggerOperation(Summary = "Check if item exists")]
    public virtual async Task<ActionResult<ApiResponse<bool>>> Exists(TKey id)
    {
        var exists = await EntityReadService.ExistsAsync(id);
        return exists
            ? Ok(ApiResponse<bool>.Ok(true))
            : NotFound(ApiResponse<bool>.Fail("Not found"));
    }

    [HttpPost("paged")]
    [SwaggerOperation(Summary = "Retrieve items with pagination")]
    public virtual async Task<ApiResponse<PaginationInfo<TReadDto>>> GetPaged([FromBody] PagedRequest request)
    {
        var paging = new PagingParameters(request.Page, request.PageSize);

        var (filter, sort) = PagedQueryBuilder.Build(
            request,
            _searchBuilder,
            _sortBuilder
        );

        filter = request.Filters.ApplyDynamicFilters(filter);

        var (items, totalCount) = await EntityReadService.GetPagedAsync(paging, filter, sort);

        var paginationInfo = new PaginationInfo<TReadDto>(paging.Page, paging.PageSize, totalCount)
        {
            Items = items.ToList()
        };

        return ApiResponse<PaginationInfo<TReadDto>>.Ok(paginationInfo);
    }

    protected void SetSearchFilterBuilder(Func<string, Expression<Func<TEntity, bool>>> builder)
    {
        _searchBuilder = builder;
    }

    protected void SetSortBuilder(Func<string?, bool, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> builder)
    {
        _sortBuilder = builder;
    }
}