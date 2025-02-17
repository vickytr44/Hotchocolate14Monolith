using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public interface IBillRepository
{
    IQueryable<Bill> GetBills();
}