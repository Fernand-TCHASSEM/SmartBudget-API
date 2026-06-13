using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class CategoryRuleRepository(SmartBudgetDbContext db) : Repository<CategoryRule>(db), ICategoryRuleRepository
{
    public async Task<PagedResponse<CategoryRule>> GetPagedForCategoryAsync(
        string userId,
        string categoryId,
        PaginationFilter filter,
        CancellationToken ct = default)
    {
        var query = Set.AsNoTracking()
            .Where(cr => cr.CategoryId == categoryId &&
                (cr.UserId == null || cr.UserId == userId));

        if (!string.IsNullOrWhiteSpace(filter.Search))
        {
            query = query.Where(c =>
                (c.Name != null && c.Name.Contains(filter.Search)) ||
                c.Keyword.Contains(filter.Search));
        }

        return await GetPagedAsync(query, filter, defaultSort: "CreatedAt desc", ct);
    }
}
