using System.Linq.Expressions;
using AutoMapper;
using Zm.Common.Interfaces;
using Zm.Common.Models;
using Zm.Controllers.Extensions;
using Zm.Controllers.Interfaces;

namespace Zm.Controllers.Services
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ReadControllerService<TEntity, TKey, TReadDto>
        : ControllerServiceBase<TEntity, TKey>, IReadOnlyService<TEntity, TKey, TReadDto>
        where TEntity : class
    {
        // ReSharper disable once ConvertToPrimaryConstructor
        public ReadControllerService(IGenericRepository<TEntity, TKey> repository, IMapper mapper) : base(repository,
            mapper)
        {
        }

        public async Task<TReadDto?> GetByIdAsync(TKey id)
        {
            var entity = await Repository.GetByIdAsync(id);
            return entity is not null ? Mapper.SafeMap<TEntity, TReadDto>(entity) : default;
        }

        public async Task<(IEnumerable<TReadDto> Items, int TotalCount)> GetPagedAsync(PagingParameters paging,
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null)
        {
            return await GetPagedInternalAsync<TReadDto>(paging, filter, sort);
        }

        public virtual async Task<bool> ExistsAsync(TKey id) => await Repository.ExistsAsync(id);
    }
}