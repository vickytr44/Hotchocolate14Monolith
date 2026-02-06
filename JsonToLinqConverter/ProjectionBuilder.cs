using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotChocolateV14.JsonToLinqConverter;

public static class ProjectionBuilder
{
    public static IQueryable ApplySelect(
    IQueryable source,
    Type entityType,
    IReadOnlyList<string> mainFields,
    IReadOnlyList<RelatedTableRequest>? relatedTables,
    DbContext dbContext)
    {
        if ((mainFields == null || mainFields.Count == 0) && (relatedTables == null || relatedTables.Count == 0))
        {
            return source;
        }

        var parameter = Expression.Parameter(entityType, "e");

        var addMethod = typeof(Dictionary<string, object>)
            .GetMethod(nameof(Dictionary<string, object>.Add))!;

        var elementInits = mainFields.Select(field =>
        {
            var property = entityType.GetProperties()
                .FirstOrDefault(p =>
                    string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new InvalidOperationException(
                    $"Property '{field}' not found on '{entityType.Name}'");

            var propertyAccess = Expression.Property(parameter, property);

            // box value types
            var valueExpr = Expression.Convert(propertyAccess, typeof(object));

            return Expression.ElementInit(
                addMethod,
                Expression.Constant(property.Name.ToLowerInvariant()),
                valueExpr);
        }).ToList();

        foreach (var related in relatedTables ?? Enumerable.Empty<RelatedTableRequest>())
        {
            var navigation = NavigationResolver.ResolveNavigation(
                dbContext,
                entityType,
                related.Entity);

            var navProperty = Expression.Property(parameter, navigation.PropertyInfo!);

            var relatedParam = Expression.Parameter(
                navigation.TargetEntityType.ClrType, "r");

            var innerSelect = BuildDictionaryProjection(
                relatedParam,
                navigation.TargetEntityType.ClrType,
                related.SelectedFields);

            var innerLambda = Expression.Lambda(innerSelect, relatedParam);

            var relatedSelectCall = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.Select),
                new[]
                {
                    navigation.TargetEntityType.ClrType,
                    typeof(Dictionary<string, object>)
                },
                navProperty,
                innerLambda);

            // .ToList()
            var toListCall = Expression.Call(
                typeof(Enumerable),
                nameof(Enumerable.ToList),
                new[] { typeof(Dictionary<string, object>) },
                relatedSelectCall);

            // add to main dictionary
            elementInits.Add(
                Expression.ElementInit(
                    addMethod,
                    Expression.Constant(navigation.Name.ToLowerInvariant()),
                    Expression.Convert(toListCall, typeof(object))
                ));
        }

        var newDictExpr = Expression.ListInit(
            Expression.New(typeof(Dictionary<string, object>)),
            elementInits);

        // Expression<Func<TEntity, Dictionary<string, object>>>
        var selector = Expression.Lambda(
            newDictExpr,
            parameter);

        // Build Queryable.Select<TEntity, Dictionary<string, object>>(source, selector)
        var selectCall = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.Select),
            new[] { entityType, typeof(Dictionary<string, object>) },
            source.Expression,
            selector);

        return source.Provider.CreateQuery(selectCall);
    }

    private static Expression BuildDictionaryProjection(
    ParameterExpression parameter,
    Type entityType,
    IReadOnlyList<string> fields)
    {
        var addMethod = typeof(Dictionary<string, object>)
            .GetMethod(nameof(Dictionary<string, object>.Add))!;

        var elementInits = new List<ElementInit>();

        foreach (var field in fields)
        {
            var property = entityType.GetProperties()
                .FirstOrDefault(p =>
                    string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new InvalidOperationException(
                    $"Property '{field}' not found on '{entityType.Name}'");

            var propertyAccess = Expression.Property(parameter, property);

            var boxedValue = Expression.Convert(propertyAccess, typeof(object));

            elementInits.Add(
                Expression.ElementInit(
                    addMethod,
                    Expression.Constant(property.Name.ToLowerInvariant()),
                    boxedValue));
        }

        return Expression.ListInit(
            Expression.New(typeof(Dictionary<string, object>)),
            elementInits);
    }
}