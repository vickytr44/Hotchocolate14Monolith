using GreenDonut.Selectors;
using HotChocolate.Pagination;
using HotChocolateV14.DataLoaders;
using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public class AccountRepository(ApplicationContext context, AccountByIdDataLoader accountById) : IAccountRepository
{
    public IQueryable<Account> GetAccounts()
    {
        return context.Accounts;
    }    
    
    public IQueryable<Account> GetAccountsById(int customerId)
    {
        return context.Accounts.Where(x => x.CustomerId == customerId);
    }

    public async Task<Page<Account>> GetAccountsById(int customerId, PagingArguments args, CancellationToken ct = default)
    {
        return await accountById.WithPagingArguments(args).LoadAsync(customerId, ct);
    }
}
