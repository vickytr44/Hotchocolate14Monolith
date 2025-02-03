using HotChocolateV14.Entities;
using HotChocolateV14.Repositories;

namespace HotChocolateV14;

[ObjectType<Customer>]
public static partial class CustomerNode
{
    //[UsePaging]
    //[UseSorting]
    //public static async Task<Connection<Account>> GetPaginatedAccounts([Parent(nameof(Customer.Id))] Customer customer, ISortingContext sorting, PagingArguments pagingArguments, IAccountRepository repository,CancellationToken cancellationToken)
    //{
    //    sorting.Handled(false);

    //    sorting.OnAfterSortingApplied<IQueryable<Account>>(
    //        static (sortingApplied, query) =>
    //        {
    //            if (sortingApplied && query is IOrderedQueryable<Account> ordered)
    //            {
    //                return ordered.ThenBy(b => b.Id);
    //            }

    //            return query.OrderBy(b => b.Id);
    //        });
    //    return await repository.GetAccountsById(customer.Id, pagingArguments, cancellationToken).ToConnectionAsync();
    //}

    [UsePaging(IncludeTotalCount = true)]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public static IQueryable<Account> GetPaginatedAccounts([Parent(nameof(Customer.Id))] Customer customer, IAccountRepository repository)
    {
        return repository.GetAccountsById(customer.Id);
    }
}
