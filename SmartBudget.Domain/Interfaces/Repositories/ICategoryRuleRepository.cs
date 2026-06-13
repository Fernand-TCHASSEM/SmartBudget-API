using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface ICategoryRuleRepository : IRepository<CategoryRule>
{
    Task<PagedResponse<CategoryRule>> GetPagedForCategoryAsync(
        string userId,
        string categoryId,
        PaginationFilter filter,
        CancellationToken ct = default);
}
