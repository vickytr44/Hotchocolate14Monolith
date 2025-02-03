using HotChocolateV14.Entities;
using HotChocolateV14.Repositories;

namespace HotChocolateV14.Queries;

[QueryType]
public class AccountQuery
{
    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Account> GetAccounts(IAccountRepository repository)
    {
        return repository.GetAccounts();
    }
}
