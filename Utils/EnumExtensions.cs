using System.ComponentModel;
using System.Reflection;

namespace HotChocolateV14.Utils;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo field = value.GetType().GetField(value.ToString());
        DescriptionAttribute attribute = field.GetCustomAttribute<DescriptionAttribute>();

        return attribute == null ? value.ToString() : attribute.Description;
    }

    public static T GetEnumValueFromDescription<T>(string description) where T : Enum
    {
        var type = typeof(T);
        foreach (var field in type.GetFields())
        {
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            if (attribute != null && attribute.Description == description)
            {
                return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException($"No enum value found for description '{description}'", nameof(description));
    }
}
