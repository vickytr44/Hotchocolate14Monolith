using HotChocolateV14.Entities;
using HotChocolateV14.Repositories;

namespace HotChocolateV14.Queries;

[QueryType]
public class CustomerQuery
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers(ICustomerRepository repository)
    {
        return repository.GetCustomers();
    }
}
