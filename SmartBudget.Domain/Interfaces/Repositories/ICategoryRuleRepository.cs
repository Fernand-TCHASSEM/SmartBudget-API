using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface ICategoryRuleRepository : IRepository<CategoryRule>
{
    Task<PagedResponse<CategoryRule>> GetPagedForCategoryAsync(
        string userId,
        string categoryId,
        PaginationFilter filter,
        bool? isRegex = null,
        CancellationToken ct = default);
}
