using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;

namespace Zm.Common.Helpers;

public static class EfHelper
{
    private static readonly ConcurrentDictionary<Type, string> PrimaryKeyCache = new();

    public static string GetPrimaryKeyName<TEntity>(DbContext ctx)
        where TEntity : class
    {
        return PrimaryKeyCache.GetOrAdd(typeof(TEntity), _ =>
        {
            var entityType = ctx.Model.FindEntityType(typeof(TEntity))
                             ?? throw new InvalidOperationException(
                                 $"Entity {typeof(TEntity).Name} not found in model");

            var pk = entityType.FindPrimaryKey()
                     ?? throw new InvalidOperationException($"Entity {typeof(TEntity).Name} has no primary key");

            if (pk.Properties.Count != 1)
                throw new NotSupportedException("Only single-column keys are supported");

            return pk.Properties[0].Name;
        });
    }
}