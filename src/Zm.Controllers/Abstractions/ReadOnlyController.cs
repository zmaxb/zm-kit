using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zm.Common.Models;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Abstractions;

[ApiController]
public abstract class ReadOnlyController<TEntity, TKey, TReadDto>(
    IEntityReadService<TEntity, TKey, TReadDto> service)
    : ControllerBase
{
    // ReSharper disable once MemberCanBePrivate.Global
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
    [HttpGet]
    [SwaggerOperation(Summary = "Get paginated list of items")]
    public virtual async Task<ActionResult<ApiResponse<PaginationInfo<TReadDto>>>> GetPaged(
        [FromQuery] PagedRequest request)
    {
        var (items, total) = await EntityReadService.GetPagedAsync(request);
        var paginationInfo = new PaginationInfo<TReadDto>(request.Page, request.PageSize, total, items.ToList());
        return ApiResponse<PaginationInfo<TReadDto>>.Ok(paginationInfo);
    }
}