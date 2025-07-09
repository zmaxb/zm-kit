using System.Linq.Expressions;

namespace Zm.Common.Helpers;

public static class QueryableSortingHelper
{
    public static IQueryable<T> Apply<T>(IQueryable<T> query, string? sortBy, bool descending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.PropertyOrField(parameter, sortBy);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = descending ? "OrderByDescending" : "OrderBy";

        var method = typeof(Queryable).GetMethods()
            .First(m => m.Name == methodName
                        && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), property.Type);

        return (IQueryable<T>)method.Invoke(null, [query, lambda])!;
    }
}