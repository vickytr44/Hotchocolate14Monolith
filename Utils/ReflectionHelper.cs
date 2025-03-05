using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using System.Collections;
using System.Reflection;

namespace HotChocolateV14.Utils;

public static class ReflectionHelper
{
    public static IEnumerable<AvailableField> GetFieldsAndTypes<T>()
    {
        var fieldsAndTypes = GetFieldNamesAndTypes<T>();

        return fieldsAndTypes.Select(fieldAndType => new AvailableField
        {
            Name = fieldAndType.Key,
            Type = fieldAndType.Value
        });
    }

    public static IEnumerable<string> GetVirtualFields<T>()
    {
        return GetVirtualFieldNames<T>();
    }

    public static List<AvailableEntity> GetRelatedEntities<T>()
    {
        var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(field => field.GetMethod != null && field.GetMethod.IsVirtual && !IsCollectionType(field.PropertyType));

        return fields.Select(field => new AvailableEntity
        {
            Id = field.Name,
            value = Enum.Parse<Entity>(field.Name).GetDescription()
        }).ToList();
    }

    public static List<AvailableEntity> GetFilterEntities<T>()
    {
        var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(field => field.GetMethod != null && field.GetMethod.IsVirtual);

        return fields.Select(field => new AvailableEntity
        {
            Id = field.Name,
            value = Enum.Parse<Entity>(field.Name).GetDescription()
        }).ToList();
    }

    private static Dictionary<string, string> GetFieldNamesAndTypes<T>()
    {
        var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(field => !field.GetMethod.IsVirtual);

        return fields.ToDictionary(field => field.Name, field => field.PropertyType.Name);
    }

    private static List<string> GetVirtualFieldNames<T>()
    {
        var fields = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                               .Where(field => field.GetMethod.IsVirtual);

        return fields.Select(field => field.Name).ToList();
    }

    private static bool IsCollectionType(Type type)
    {
        if (typeof(ICollection).IsAssignableFrom(type))
        {
            return true;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
        {
            return true;
        }

        return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICollection<>));
    }
}