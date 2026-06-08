using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class CategoryRepository(SmartBudgetDbContext db) : Repository<Category>(db), ICategoryRepository
{
    public Task<IEnumerable<Category>> GetAllForUserAsync(string userId, CancellationToken ct = default) =>
        FindAllByAsync(c => c.UserId == userId || c.IsDefault, ct);
}
