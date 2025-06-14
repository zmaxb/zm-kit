using System.Linq.Expressions;
using System.Reflection;
using Zm.Common.Models;

namespace Zm.Controllers.Extensions;

public static class PagedQueryBuilder
{
    public static (Expression<Func<TEntity, bool>>? Filter,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? Sort)
        Build<TEntity>(
            PagedRequest request,
            Func<string, Expression<Func<TEntity, bool>>>? searchBuilder = null,
            Func<string?, bool, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>? sortBuilder = null)
    {
        Expression<Func<TEntity, bool>>? filter = null;

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            if (searchBuilder is not null)
            {
                filter = searchBuilder(request.Search);
            }
            else
            {
                var param = Expression.Parameter(typeof(TEntity), "x");
                var prop = typeof(TEntity).GetProperties()
                    .FirstOrDefault(p => p.PropertyType == typeof(string));

                if (prop != null)
                {
                    var propAccess = Expression.Property(param, prop);
                    var search = Expression.Constant(request.Search);
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                    var body = Expression.Call(propAccess, containsMethod, search);
                    filter = Expression.Lambda<Func<TEntity, bool>>(body, param);
                }
            }
        }

        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? sort = null;

        if (sortBuilder is not null)
        {
            sort = sortBuilder(request.SortBy, request.Descending);
        }
        else if (!string.IsNullOrWhiteSpace(request.SortBy))
        {
            var prop = typeof(TEntity).GetProperty(request.SortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (prop == null) return (filter, sort);
            
            var param = Expression.Parameter(typeof(TEntity), "x");
            var body = Expression.Property(param, prop);
            var lambda = Expression.Lambda(body, param);

            sort = q =>
            {
                var methodName = request.Descending ? "OrderByDescending" : "OrderBy";
                var method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), prop.PropertyType);

                var result = method.Invoke(null, [q, lambda])!;
                return (IOrderedQueryable<TEntity>)result;
            };
        }

        return (filter, sort);
    }
}