using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public class CustomerRepository(ApplicationContext context) : ICustomerRepository
{
    public IQueryable<Customer> GetCustomers()
    {
        return context.Customers;
    }
}