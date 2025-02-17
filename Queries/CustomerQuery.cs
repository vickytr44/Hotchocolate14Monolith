using HotChocolateV14.Entities;
using HotChocolateV14.Repositories;

namespace HotChocolateV14.Queries;

[QueryType]
public class CustomerQuery
{
    [UsePaging(IncludeTotalCount = true,DefaultPageSize = 10000, MaxPageSize = 10000)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Customer> GetCustomers(ICustomerRepository repository)
    {
        return repository.GetCustomers();
    }
}
