using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;
using System.Reflection;

namespace HotChocolateV14.Queries
{
    //[QueryType]
    public class AvailableFieldQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableField> GetAvailableFields(string entity)
        {
            var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);

            Type type = Assembly.GetExecutingAssembly()
                                  .GetTypes()
                                  .First(t => t.Name == entityEnum.ToString() && t.Namespace == "HotChocolateV14.Entities");

            var result = typeof(ReflectionHelper).GetMethod(nameof(ReflectionHelper.GetFieldsAndTypes))
                                                   .MakeGenericMethod(type).Invoke(null, null) as IEnumerable<AvailableField> ?? [];


            return result;
        }

        //private static List<AvailableField> GetFieldsForEntity<T>(string json, string entity)
        //{
        //    var fields = JsonHelper.ExtractAvailableFields(json, entity);

        //    var virtualFields = ReflectionHelper.GetVirtualFields<T>();

        //    return fields.Where(x => !virtualFields.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).ToList();
        //}
    }
}
