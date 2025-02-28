using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories
{
    public interface IAvailableEntityRepository
    {
        IQueryable<AvailableEntity> GetAvailableEntities();
    }
}
