using System.Linq.Expressions;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<bool> ExistsByIdAsync(string id, CancellationToken ct = default);
    Task<IEnumerable<T>> FindAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<T?> FindByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
    Task<bool> ExistsByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
}
