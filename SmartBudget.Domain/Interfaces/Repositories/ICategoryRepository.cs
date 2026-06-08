using SmartBudget.Domain.Entities;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetAllForUserAsync(string userId, CancellationToken ct = default);
}
