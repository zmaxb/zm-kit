using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;

namespace Zm.Controllers.Extensions;

public static class FilterExpressionBuilder
{
    public static Expression<Func<T, bool>> Build<T>(Dictionary<string, object> filters)
    {
        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? body = null;

        foreach (var (key, value) in filters)
        {
            var propInfo = typeof(T).GetProperty(key,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (propInfo == null) continue;

            var left = Expression.Property(parameter, propInfo);

            object? typedValue;
            try
            {
                if (value is JsonElement json)
                {
                    if (propInfo.PropertyType.IsEnum)
                        typedValue = Enum.Parse(propInfo.PropertyType, json.GetString()!, true);
                    else if (propInfo.PropertyType == typeof(string))
                        typedValue = json.GetString();
                    else if (propInfo.PropertyType == typeof(int))
                        typedValue = json.GetInt32();
                    else if (propInfo.PropertyType == typeof(Guid))
                        typedValue = json.GetGuid();
                    else if (propInfo.PropertyType == typeof(bool))
                        typedValue = json.GetBoolean();
                    else if (propInfo.PropertyType == typeof(DateTime))
                        typedValue = json.GetDateTime();
                    else
                        typedValue = Convert.ChangeType(json.ToString(), propInfo.PropertyType);
                }
                else
                {
                    typedValue = Convert.ChangeType(value, propInfo.PropertyType);
                }
            }
            catch
            {
                continue;
            }

            var right = Expression.Constant(typedValue, propInfo.PropertyType);
            var equals = Expression.Equal(left, right);

            body = body == null ? equals : Expression.AndAlso(body, equals);
        }

        return Expression.Lambda<Func<T, bool>>(body ?? Expression.Constant(true), parameter);
    }

    public static Expression<Func<T, bool>>? ApplyDynamicFilters<T>(
        this Dictionary<string, object>? filters,
        Expression<Func<T, bool>>? existingFilter = null)
    {
        if (filters == null || filters.Count == 0)
            return existingFilter;

        var dynamicFilter = Build<T>(filters);
        return existingFilter is null
            ? dynamicFilter
            : existingFilter.AndAlso(dynamicFilter);
    }
}

public static class ExpressionExtensions
{
    public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> expr1,
        Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceParameterVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceParameterVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(left, right), parameter);
    }

    private class ReplaceParameterVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        : ExpressionVisitor
    {
        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == oldParameter ? newParameter : base.VisitParameter(node);
        }
    }
}