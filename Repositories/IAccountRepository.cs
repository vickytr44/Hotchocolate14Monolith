﻿using HotChocolate.Pagination;
using HotChocolateV14.Entities;

namespace HotChocolateV14.Repositories;

public interface IAccountRepository
{
    IQueryable<Account> GetAccounts();

    IQueryable<Account> GetAccountsById(int customerId);
}
