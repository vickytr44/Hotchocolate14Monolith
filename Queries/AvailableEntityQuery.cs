using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;

namespace HotChocolateV14.Queries
{
    [QueryType]
    public class AvailableEntityQuery
    {
        //[UseProjection]
        //[UseFiltering]
        //[UseSorting]
        //public IQueryable<AvailableEntity> GetAvailableEntities(IAvailableEntityRepository repository)
        //{
        //    return repository.GetAvailableEntities();
        //}
        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableEntity> GetAvailableEntities()
        {
            return [
                new AvailableEntity() { Id = Entity.Customer.ToString() , value = Entity.Customer.GetDescription()},
                new AvailableEntity() { Id = Entity.Account.ToString() , value = Entity.Account.GetDescription()},
                new AvailableEntity() { Id = Entity.Bill.ToString() , value = Entity.Bill.GetDescription()}
            ];
        }

        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableEntity> GetAvailableRelatedEntities(string entity)
        {
            var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);
            return entityEnum switch
            {
                Entity.Customer => ReflectionHelper.GetRelatedEntities<Customer>(),
                Entity.Bill => ReflectionHelper.GetRelatedEntities<Bill>(),
                Entity.Account => ReflectionHelper.GetRelatedEntities<Account>(),
                _ => [],
            };
        }
    }
}
