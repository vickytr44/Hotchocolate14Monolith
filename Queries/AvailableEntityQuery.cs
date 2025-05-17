using HotChocolateV14.Constants;
using HotChocolateV14.Entities;
using HotChocolateV14.Utils;
using System.Reflection;

namespace HotChocolateV14.Queries
{
    //[QueryType]
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
            var availableEntities = new List<AvailableEntity>();

            foreach(var entity in Enum.GetValues(typeof(Entity)).Cast<Entity>()){
                availableEntities.Add(new AvailableEntity() { Id = entity.ToString(), value = entity.GetDescription() });
            }

            return availableEntities;
        }

        [UseFiltering]
        [UseSorting]
        public IEnumerable<AvailableEntity> GetAvailableRelatedEntities(string entity)
        {
            var entityEnum = EnumExtensions.GetEnumValueFromDescription<Entity>(entity);

            Type type = Assembly.GetExecutingAssembly()
                                  .GetTypes()
                                  .First(t => t.Name == entityEnum.ToString() && t.Namespace == "HotChocolateV14.Entities");

            var result = typeof(ReflectionHelper).GetMethod(nameof(ReflectionHelper.GetRelatedEntities))
                                                   .MakeGenericMethod(type).Invoke(null, null) as List<AvailableEntity> ?? [];

            return result;
        }
    }
}
