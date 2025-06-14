using System.Linq.Expressions;
using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Services;

public class DetailedReadControllerService<TEntity, TDetailedDto, TKey>
    : ControllerServiceBase<TEntity, TKey>, IDetailedReadService<TEntity, TDetailedDto, TKey>
    where TEntity : class
{
    // ReSharper disable once ConvertToPrimaryConstructor
    public DetailedReadControllerService(IGenericRepository<TEntity, TKey> repository, IMapper mapper)
        : base(repository, mapper)
    {
    }

    public async Task<TDetailedDto?> GetDetailedByIdAsync(TKey id)
    {
        var entity = await Repository.GetByIdAsync(id);
        return entity is not null
            ? Mapper.SafeMap<TEntity, TDetailedDto>(entity)
            : default;
    }

    public async Task<(IEnumerable<TDetailedDto> Items, int TotalCount)> GetPagedDetailedAsync(
        PagingParameters paging,
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null)
    {
        return await GetPagedInternalAsync<TDetailedDto>(paging, filter, sort);
    }
}