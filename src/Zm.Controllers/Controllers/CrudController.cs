using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Zm.Common.Models;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Controllers;

public abstract class CrudController<TEntity, TKey, TReadDto, TCreateDto, TUpdateDto>(
    IReadOnlyService<TEntity, TKey, TReadDto> readOnlyService,
    IModifiableService<TCreateDto, TUpdateDto, TKey> modifiableService)
    : ReadOnlyController<TEntity, TKey, TReadDto>(readOnlyService)
    where TEntity : class
{
    protected IModifiableService<TCreateDto, TUpdateDto, TKey> ModifiableService { get; } = modifiableService;

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new item")]
    public async Task<ActionResult<ApiResponse<TKey>>> Create([FromBody] TCreateDto dto)
    {
        var id = await ModifiableService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<TKey>.Ok(id, "Successfully created"));
    }

    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete an item by ID")]
    public async Task<ActionResult<ApiResponse<int>>> Delete(TKey id)
        => await ModifiableService.DeleteAsync(id) is var deletedCount and > 0
            ? Ok(ApiResponse<int>.Ok(deletedCount, "Successfully deleted"))
            : NotFound(ApiResponse<string>.Fail("Not found"));


    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update an existing item")]
    public async Task<ActionResult<ApiResponse<bool>>> Update(TKey id, [FromBody] TUpdateDto dto)
        => await ModifiableService.UpdateAsync(dto, id)
            ? Ok(ApiResponse<bool>.Ok(true, "Successfully updated"))
            : NotFound(ApiResponse<bool>.Fail("Not found"));
}