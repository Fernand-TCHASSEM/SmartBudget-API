using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class CategoryRepository(SmartBudgetDbContext db) : Repository<Category>(db), ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllForUserAsync(string userId, CancellationToken ct = default) =>
        FindAllByAsync(c => c.UserId == userId || c.IsDefault, ct);

    public async Task<PagedResponse<Category>> GetPagedForUserAsync(
        string userId,
        PaginationFilter filter,
        bool? isIncome = null,
        bool? isDefault = null,
        CancellationToken ct = default)
    {
        var query = Set.AsNoTracking()
            .Where(c => c.UserId == userId || c.IsDefault);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(c => c.Name.Contains(filter.Search));

        if (isIncome.HasValue)
            query = query.Where(c => c.IsIncome == isIncome.Value);

        if (isDefault.HasValue)
            query = query.Where(c => c.IsDefault == isDefault.Value);

        return await GetPagedAsync(query, filter, defaultSort: "SortOrder asc, Name asc", ct);
    }
}
