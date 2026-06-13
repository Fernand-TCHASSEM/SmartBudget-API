using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllForUserAsync(string userId, CancellationToken ct = default);

    Task<PagedResponse<Category>> GetPagedForUserAsync(
        string userId,
        PaginationFilter filter,
        bool? isIncome = null,
        bool? isDefault = null,
        CancellationToken ct = default);
}
