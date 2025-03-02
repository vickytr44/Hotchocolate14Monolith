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
            var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>( entity);
            return entityEnum switch
            {
                Entity.Customer => ReflectionHelper.GetFieldsAndTypes<Customer>(),
                Entity.Bill => ReflectionHelper.GetFieldsAndTypes<Bill>(),
                Entity.Account => ReflectionHelper.GetFieldsAndTypes<Account>(),
                _ => [],
            };
        }
    }
}
