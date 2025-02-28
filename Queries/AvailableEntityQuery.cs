using HotChocolateV14.Entities;

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

        [UseProjection]
        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableEntity> GetAvailableEntities()
        {
            return [
                new AvailableEntity() { Id = "Customer", value = "customers" },
                new AvailableEntity()  { Id = "Account", value = "accounts" },
                new AvailableEntity()  { Id = "Bill", value = "bills" }
            ];
        }
    }
}
