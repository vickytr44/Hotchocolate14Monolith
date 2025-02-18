using GreenDonut.Selectors;
using HotChocolate.Pagination;
using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public class AccountRepository(ApplicationContext context) : IAccountRepository
{
    public IQueryable<Account> GetAccounts()
    {
        return context.Accounts;
    }    
    
    public IQueryable<Account> GetAccountsById(int customerId)
    {
        return context.Accounts.Where(x => x.CustomerId == customerId);
    }
}
