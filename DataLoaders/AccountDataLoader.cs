using GreenDonut.Selectors;
using HotChocolate.Pagination;
using HotChocolateV14.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace HotChocolateV14.DataLoaders;

public static class AccountDataLoader
{
    [DataLoader]
    public static async Task<Dictionary<int, Page<Account>>> GetAccountById(
    IReadOnlyList<int> keys,
    PagingArguments pagingArguments,
    ApplicationContext context,
    CancellationToken ct)
    {
        return await context.Accounts
            .AsNoTracking()
            .Where(p => keys.Contains(p.CustomerId))
            //.OrderBy(p => p.CustomerId)
            .ToBatchPageAsync(t => t.CustomerId, pagingArguments, ct);
    }
}


