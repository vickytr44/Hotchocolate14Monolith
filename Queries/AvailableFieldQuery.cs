using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;

namespace HotChocolateV14.Queries
{
    [QueryType]
    public class AvailableFieldQuery
    {
        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableField> GetAvailableFields(string entity)
        {
            var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);
            return entityEnum switch
            {
                Entity.Customer => ReflectionHelper.GetFieldsAndTypes<Customer>(),
                Entity.Bill => ReflectionHelper.GetFieldsAndTypes<Bill>(),
                Entity.Account => ReflectionHelper.GetFieldsAndTypes<Account>(),
                _ => [],
            };

            //string filePath = ".\\Jsons\\types.json";
            //string json = File.ReadAllText(filePath);

            //return entityEnum switch
            //{
            //    Entity.Customer => GetFieldsForEntity<Customer>(json, Entity.Customer.ToString()),
            //    Entity.Bill => GetFieldsForEntity<Bill>(json, Entity.Bill.ToString()),
            //    Entity.Account => GetFieldsForEntity<Account>(json, Entity.Account.ToString()),
            //    _ => [],
            //};
        }

        //private static List<AvailableField> GetFieldsForEntity<T>(string json, string entity)
        //{
        //    var fields = JsonHelper.ExtractAvailableFields(json, entity);

        //    var virtualFields = ReflectionHelper.GetVirtualFields<T>();

        //    return fields.Where(x => !virtualFields.Contains(x.Name, StringComparer.OrdinalIgnoreCase)).ToList();
        //}
    }
}
