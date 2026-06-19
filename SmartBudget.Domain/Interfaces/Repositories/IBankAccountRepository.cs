using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface IBankAccountRepository : IRepository<BankAccount>
{
    Task<PagedResponse<BankAccount>> GetPagedForUserAsync(
        string userId,
        PaginationFilter filter,
        AccountType? accountType = null,
        Currency? currency = null,
        CancellationToken ct = default);
}
