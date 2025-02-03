using HotChocolate.Execution.Processing;
using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public interface ICustomerRepository
{
    IQueryable<Customer> GetCustomers();
}