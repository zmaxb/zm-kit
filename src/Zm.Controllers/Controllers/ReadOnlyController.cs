using System.Linq.Expressions;
using D3us.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zm.Common.Models;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Controllers;

[ApiController]
public abstract class ReadOnlyController<TEntity, TKey, TReadDto>(
    IReadOnlyService<TEntity, TKey, TReadDto> service)
    : ControllerBase
{
    protected IReadOnlyService<TEntity, TKey, TReadDto> ReadOnlyService { get; } = service;

    private Func<string, Expression<Func<TEntity, bool>>>? _searchBuilder;
    private Func<string?, bool, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? _sortBuilder;

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve item by ID")]
    public async Task<ActionResult<ApiResponse<TReadDto>>> GetById(TKey id)
        => await ReadOnlyService.GetByIdAsync(id) is { } item
            ? Ok(ApiResponse<TReadDto>.Ok(item))
            : NotFound(ApiResponse<string>.Fail("Not found"));

    [HttpGet("{id}/exists")]
    [SwaggerOperation(Summary = "Check if item exists")]
    public async Task<ActionResult<ApiResponse<bool>>> Exists(TKey id)
    {
        var exists = await ReadOnlyService.ExistsAsync(id);
        return exists
            ? Ok(ApiResponse<bool>.Ok(true))
            : NotFound(ApiResponse<bool>.Fail("Not found"));
    }

    [HttpPost("paged")]
    [SwaggerOperation(Summary = "Retrieve items with pagination")]
    public async Task<ApiResponse<PaginationInfo<TReadDto>>> GetPaged([FromBody] PagedRequest request)
    {
        var paging = new PagingParameters(request.Page, request.PageSize);

        var filter = !string.IsNullOrWhiteSpace(request.Search) && _searchBuilder != null
            ? _searchBuilder(request.Search)
            : null;
        
        filter = request.Filters.ApplyDynamicFilters<TEntity>(filter);

        var sort = _sortBuilder?.Invoke(request.SortBy, request.Descending);

        var (items, totalCount) = await ReadOnlyService.GetPagedAsync(paging, filter, sort);

        var paginationInfo = new PaginationInfo<TReadDto>(paging.Page, paging.PageSize, totalCount)
        {
            Items = items.ToList()
        };
        
        return ApiResponse<PaginationInfo<TReadDto>>.Ok(paginationInfo);
    }

    protected void SetSearchFilterBuilder(Func<string, Expression<Func<TEntity, bool>>> builder)
        => _searchBuilder = builder;

    protected void SetSortBuilder(Func<string?, bool, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> builder)
        => _sortBuilder = builder;
}