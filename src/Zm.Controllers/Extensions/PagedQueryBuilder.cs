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
                var searchParam = Expression.Parameter(typeof(TEntity), "x");
                var prop = typeof(TEntity).GetProperties()
                    .FirstOrDefault(p => p.PropertyType == typeof(string));

                if (prop != null)
                {
                    var propAccess = Expression.Property(searchParam, prop);
                    var search = Expression.Constant(request.Search);
                    var containsMethod = typeof(string).GetMethod(nameof(string.Contains), [typeof(string)])!;
                    var body = Expression.Call(propAccess, containsMethod, search);
                    filter = Expression.Lambda<Func<TEntity, bool>>(body, searchParam);
                }
            }
        }

        if (request.Filters is { Count: > 0 })
        {
            var filterParam = Expression.Parameter(typeof(TEntity), "x");
            Expression? filtersExpr = null;

            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var kv in request.Filters)
            {
                var prop = typeof(TEntity).GetProperty(kv.Key,
                    BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop is null)
                    continue;

                var value = ConvertToPropertyType(kv.Value, prop.PropertyType);

                var left = Expression.Property(filterParam, prop);
                var right = Expression.Constant(value, prop.PropertyType);
                var equalExpr = Expression.Equal(left, right);

                filtersExpr = filtersExpr is null ? equalExpr : Expression.AndAlso(filtersExpr, equalExpr);
            }

            if (filtersExpr is not null)
            {
                var filtersLambda = Expression.Lambda<Func<TEntity, bool>>(filtersExpr, filterParam);

                if (filter is null)
                {
                    filter = filtersLambda;
                }
                else
                {
                    var combinedParam = Expression.Parameter(typeof(TEntity), "x");
                    var invokedExpr1 = Expression.Invoke(filter, combinedParam);
                    var invokedExpr2 = Expression.Invoke(filtersLambda, combinedParam);
                    var combined = Expression.AndAlso(invokedExpr1, invokedExpr2);
                    filter = Expression.Lambda<Func<TEntity, bool>>(combined, combinedParam);
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

            var sortParam = Expression.Parameter(typeof(TEntity), "x");
            var body = Expression.Property(sortParam, prop);
            var lambda = Expression.Lambda(body, sortParam);

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

    private static object ConvertToPropertyType(object input, Type targetType)
    {
        try
        {
            var type = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (input is string str && type == typeof(bool))
                return bool.Parse(str);

            return Convert.ChangeType(input, type);
        }
        catch
        {
            return Activator.CreateInstance(targetType)!;
        }
    }
}