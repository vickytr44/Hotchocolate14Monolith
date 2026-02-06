using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;

namespace HotChocolateV14.JsonToLinqConverter;

/// <summary>
/// Filtering on collection navigations is not supported yet
/// </summary>
public static class WhereBuilder
{
    public static IQueryable ApplyWhere(
        IQueryable source,
        Type entityType,
        IReadOnlyList<AndCondition>? conditions,
        DbContext dbContext)
    {
        if (conditions == null || conditions.Count == 0)
            return source;

        var parameter = Expression.Parameter(entityType, "e");
        Expression? combined = null;

        foreach (var condition in conditions)
        {
            Expression left;

            // Root entity filter
            if (string.Equals(condition.Entity, entityType.Name, StringComparison.OrdinalIgnoreCase))
            {
                left = BuildPropertyAccess(parameter, entityType, condition.Field);
            }
            // Related entity filter (many-to-one / 1-1)
            else
            {
                var navigation = NavigationResolver.ResolveNavigation(
                    dbContext,
                    entityType,
                    condition.Entity);

                if (navigation.IsCollection)
                    throw new InvalidOperationException(
                        "Filtering on collection navigations is not supported yet");

                var navAccess = Expression.Property(parameter, navigation.PropertyInfo!);
                left = BuildPropertyAccess(
                    navAccess,
                    navigation.TargetEntityType.ClrType,
                    condition.Field);
            }

            var predicate = BuildOperation(left, condition.Operation, condition.Value);
            combined = combined == null
                ? predicate
                : Expression.AndAlso(combined, predicate);
        }

        var lambda = Expression.Lambda(combined!, parameter);

        var whereCall = Expression.Call(
            typeof(Queryable),
            nameof(Queryable.Where),
            new[] { entityType },
            source.Expression,
            lambda);

        return source.Provider.CreateQuery(whereCall);
    }

    private static Expression BuildPropertyAccess(
        Expression instance,
        Type type,
        string field)
    {
        var property = type.GetProperties()
            .FirstOrDefault(p =>
                string.Equals(p.Name, field, StringComparison.OrdinalIgnoreCase));

        if (property == null)
            throw new InvalidOperationException(
                $"Property '{field}' not found on '{type.Name}'");

        return Expression.Property(instance, property);
    }

    private static Expression BuildOperation(
    Expression left,
    string operation,
    object? value)
    {
        var targetType = Nullable.GetUnderlyingType(left.Type) ?? left.Type;

        object? convertedValue = value switch
        {
            JsonElement je => ConvertFromJsonElement(je, targetType),
            _ => ConvertValue(value, targetType)
        };

        var constant = Expression.Constant(convertedValue, left.Type);

        return operation.ToLowerInvariant() switch
        {
            "eq" => Expression.Equal(left, constant),
            "neq" => Expression.NotEqual(left, constant),
            "gt" => Expression.GreaterThan(left, constant),
            "gte" => Expression.GreaterThanOrEqual(left, constant),
            "lt" => Expression.LessThan(left, constant),
            "lte" => Expression.LessThanOrEqual(left, constant),

            "contains" => Expression.Call(
                left,
                nameof(string.Contains),
                Type.EmptyTypes,
                constant),

            _ => throw new NotSupportedException($"Operation '{operation}' is not supported")
        };
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null)
            return null;

        if (targetType.IsEnum)
            return Enum.Parse(targetType, value.ToString()!, ignoreCase: true);

        return Convert.ChangeType(value, targetType);
    }

    private static object? ConvertFromJsonElement(JsonElement element, Type targetType)
    {
        if (targetType == typeof(string))
            return element.GetString();

        if (targetType == typeof(int))
            return element.GetInt32();

        if (targetType == typeof(long))
            return element.GetInt64();

        if (targetType == typeof(decimal))
            return element.GetDecimal();

        if (targetType == typeof(bool))
        {
            switch (element.ValueKind)
            {
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return element.GetBoolean();

                case JsonValueKind.Number:
                    return Convert.ChangeType(
                        element.GetDecimal(), targetType);

                case JsonValueKind.String
                    when bool.TryParse(element.GetString(), out var boolValue):
                    return boolValue;
            }
        }

        if (targetType == typeof(Guid))
            return element.GetGuid();

        if (targetType.IsEnum)
            return Enum.Parse(targetType, element.GetString()!, ignoreCase: true);

        throw new NotSupportedException($"Unsupported filter value type '{targetType.Name}'");
    }
}
