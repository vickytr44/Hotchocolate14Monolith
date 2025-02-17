using HotChocolateV14.Entities;
using HotChocolateV14.Repositories;

namespace HotChocolateV14.Queries;

[QueryType]
public class AccountQuery
{
    [UsePaging(IncludeTotalCount = true, DefaultPageSize = 10000, MaxPageSize = 10000)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetAccounts(IAccountRepository repository)
    {
        return repository.GetAccounts();
    }
}
