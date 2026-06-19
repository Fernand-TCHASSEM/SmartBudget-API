using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class BankAccountRepository(SmartBudgetDbContext db) : Repository<BankAccount>(db), IBankAccountRepository
{
    public async Task<PagedResponse<BankAccount>> GetPagedForUserAsync(
        string userId,
        PaginationFilter filter,
        AccountType? accountType = null,
        Currency? currency = null,
        CancellationToken ct = default)
    {
        var query = Set.AsNoTracking()
            .Where(ba => ba.UserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(ba => ba.Name.Contains(filter.Search) ||
                ba.BankName.Contains(filter.Search));

        if (accountType.HasValue)
            query = query.Where(ba => ba.AccountType == accountType.Value);

        if (currency.HasValue)
            query = query.Where(ba => ba.Currency == currency.Value);

        return await GetPagedAsync(query, filter, defaultSort: "CreatedAt asc, Name asc", ct);
    }
}
