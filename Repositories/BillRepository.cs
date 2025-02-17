using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public class BillRepository(ApplicationContext context) : IBillRepository
{
    public IQueryable<Bill> GetBills()
    {
        return context.Bills;
    }
}