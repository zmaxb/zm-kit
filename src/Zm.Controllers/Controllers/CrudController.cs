using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zm.Common.Models;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Controllers;

public abstract class CrudController<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>(
    IEntityReadService<TEntity, TKey, TReadDto> entityReadService,
    IEntityCrudService<TCreateDto, TUpdateDto, TKey> entityCrudService)
    : ReadOnlyController<TEntity, TKey, TReadDto>(entityReadService)
    where TEntity : class
{
    private IEntityCrudService<TCreateDto, TUpdateDto, TKey> EntityCrudService { get; } = entityCrudService;

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new item")]
    public virtual async Task<ActionResult<ApiResponse<TKey>>> Create([FromBody] TCreateDto dto)
    {
        var id = await EntityCrudService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<TKey>.Ok(id, "Successfully created"));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an item by ID")]
    public virtual async Task<ActionResult<ApiResponse<int>>> Delete(TKey id)
    {
        return await EntityCrudService.DeleteAsync(id) is var deletedCount and > 0
            ? Ok(ApiResponse<int>.Ok(deletedCount, "Successfully deleted"))
            : NotFound(ApiResponse<string>.Fail("Not found"));
    }


    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing item")]
    public virtual async Task<ActionResult<ApiResponse<bool>>> Update(TKey id, [FromBody] TUpdateDto dto)
    {
        return await EntityCrudService.UpdateAsync(dto, id)
            ? Ok(ApiResponse<bool>.Ok(true, "Successfully updated"))
            : NotFound(ApiResponse<bool>.Fail("Not found"));
    }
}