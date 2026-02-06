using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace HotChocolateV14.JsonToLinqConverter;

public static class ProjectionBuilder
{
    public static IQueryable ApplySelect(
        IQueryable source,
        Type entityType,
        IReadOnlyList<string>? mainFields,
        IReadOnlyList<RelatedTableRequest>? relatedTables,
        DbContext dbContext)
    {
        mainFields ??= Array.Empty<string>();
        relatedTables ??= Array.Empty<RelatedTableRequest>();

        if (mainFields.Count == 0 && relatedTables.Count == 0)
            return source;

        var parameter = Expression.Parameter(entityType, "e");

        var addMethod = typeof(Dictionary<string, object>)
            .GetMethod(nameof(Dictionary<string, object>.Add));

        var elementInits = new List<ElementInit>();

        // -----------------------------
        // Main entity fields
        // -----------------------------

        HandleMainEntityFields(entityType, mainFields, parameter, addMethod, elementInits);

        // -----------------------------
        // Related tables
        // -----------------------------
        HandleRelatedTablesAndFields(entityType, relatedTables, dbContext, parameter, addMethod, elementInits);

        // -----------------------------
        // Build final selector
        // -----------------------------
        var selector = Expression.Lambda(
            Expression.ListInit(
                Expression.New(typeof(Dictionary<string, object>)),
                elementInits),
            parameter);

        var selectExpr = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.Select),
            new[] { entityType, typeof(Dictionary<string, object>) },
            source.Expression,
            selector);

        return source.Provider.CreateQuery(selectExpr);
    }

    // Builds expression-tree equivalents of nested projections for navigation properties.
    //
    // Collection navigation (1–many):
    // e => new Dictionary<string, object>
    // {
    //     ["orders"] = e.Orders
    //         .Select(r => new Dictionary<string, object>
    //         {
    //             ["id"] = (object)r.Id,
    //             ["amount"] = (object)r.Amount
    //         })
    //         .ToList()
    // }
    //
    // Reference navigation (many–1 / 1–1):
    // e => new Dictionary<string, object>
    // {
    //     ["customer"] = new Dictionary<string, object>
    //     {
    //         ["id"] = (object)e.Customer.Id,
    //         ["name"] = (object)e.Customer.Name
    //     }
    // }
    //
    // Each ElementInit corresponds to a Dictionary.Add(key, value) call.
    private static void HandleRelatedTablesAndFields(Type entityType, IReadOnlyList<RelatedTableRequest> relatedTables, DbContext dbContext, ParameterExpression parameter, System.Reflection.MethodInfo? addMethod, List<ElementInit> elementInits)
    {
        foreach (var related in relatedTables)
        {
            var navigation = NavigationResolver.ResolveNavigation(
                dbContext,
                entityType,
                related.Entity);

            // 1–many (collection navigation)
            if (navigation.IsCollection)
            {
                var navProperty = Expression.Property(parameter, navigation.PropertyInfo!);

                var relatedParam = Expression.Parameter(
                    navigation.TargetEntityType.ClrType, "r");

                var innerProjection = BuildDictionaryProjection(
                    relatedParam,
                    navigation.TargetEntityType.ClrType,
                    related.SelectedFields);

                var innerLambda = Expression.Lambda(innerProjection, relatedParam);

                // r => ...
                var selectCall = Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.Select),
                    new[]
                    {
                        navigation.TargetEntityType.ClrType,
                        typeof(Dictionary<string, object>)
                    },
                    navProperty,
                    innerLambda);

                var toListCall = Expression.Call(
                    typeof(Enumerable),
                    nameof(Enumerable.ToList),
                    new[] { typeof(Dictionary<string, object>) },
                    selectCall);

                elementInits.Add(
                    Expression.ElementInit(
                        addMethod,
                        Expression.Constant(navigation.Name.ToLowerInvariant()),
                        Expression.Convert(toListCall, typeof(object))));
            }
            // many–1 or 1–1 (reference navigation)
            else
            {
                var navAccess = Expression.Property(parameter, navigation.PropertyInfo!);

                var nestedProjection = BuildDictionaryProjection(
                    navAccess,
                    navigation.TargetEntityType.ClrType,
                    related.SelectedFields);

                elementInits.Add(
                    Expression.ElementInit(
                        addMethod,
                        Expression.Constant(navigation.Name.ToLowerInvariant()),
                        Expression.Convert(nestedProjection, typeof(object))));
            }
        }
    }


    // Helps to build Expression-tree equivalent of:
    // DbContext.Set<TEntity>()
    //   .Select(e => new Dictionary<string, object>
    //   {
    //       ["field1"] = (object)e.Field1,
    //       ["field2"] = (object)e.Field2
    //   })
    //
    // Each ElementInit represents a Dictionary.Add(key, value) call.

    private static void HandleMainEntityFields(Type entityType, IReadOnlyList<string> mainFields, ParameterExpression parameter, System.Reflection.MethodInfo? addMethod, List<ElementInit> elementInits)
    {
        foreach (var field in mainFields)
        {
            var property = entityType.GetProperties()
                .FirstOrDefault(p =>
                    string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new InvalidOperationException(
                    $"Property '{field}' not found on '{entityType.Name}'");

            elementInits.Add(
                Expression.ElementInit(
                    addMethod,
                    Expression.Constant(property.Name.ToLowerInvariant()),
                    Expression.Convert(
                        Expression.Property(parameter, property),
                        typeof(object))));
        }
    }

    // ------------------------------------------------------------
    // Builds: new Dictionary<string, object> { ... }
    // ------------------------------------------------------------
    private static Expression BuildDictionaryProjection(
        Expression instance,
        Type entityType,
        IReadOnlyList<string> fields)
    {
        if (fields == null || fields.Count == 0)
            throw new InvalidOperationException(
                $"No fields selected for '{entityType.Name}'");

        var addMethod = typeof(Dictionary<string, object>)
            .GetMethod(nameof(Dictionary<string, object>.Add))!;

        var inits = new List<ElementInit>();

        foreach (var field in fields)
        {
            var property = entityType.GetProperties()
                .FirstOrDefault(p =>
                    string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

            if (property == null)
                throw new InvalidOperationException(
                    $"Property '{field}' not found on '{entityType.Name}'");

            // Prevent accidental deep joins
            if (!property.PropertyType.IsValueType &&
                property.PropertyType != typeof(string))
                throw new InvalidOperationException(
                    $"Navigation property '{property.Name}' is not allowed in projection");

            inits.Add(
                Expression.ElementInit(
                    addMethod,
                    Expression.Constant(property.Name.ToLowerInvariant()),
                    Expression.Convert(
                        Expression.Property(instance, property),
                        typeof(object))));
        }

        return Expression.ListInit(
            Expression.New(typeof(Dictionary<string, object>)),
            inits);
    }
}