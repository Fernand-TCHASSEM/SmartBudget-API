using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;
using SmartBudget.Infrastructure.Persistence;
using SmartBudget.Infrastructure.Persistence.Extensions;

namespace SmartBudget.Infrastructure.Repositories;

public abstract class Repository<T>(SmartBudgetDbContext db) : IRepository<T> where T : class
{
    protected readonly SmartBudgetDbContext Db = db;
    protected readonly DbSet<T> Set = db.Set<T>();

    public virtual async Task<T?> GetByIdAsync(string id, CancellationToken ct = default) =>
        await Set.FindAsync([id], ct);

    public virtual async Task<bool> ExistsByIdAsync(string id, CancellationToken ct = default) =>
        await Set.FindAsync([id], ct) is not null;

    public virtual async Task<IEnumerable<T>> FindAllByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await Set.Where(predicate).ToListAsync(ct);

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        Set.Add(entity);
        await Db.SaveChangesAsync(ct);
        return entity;
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        Set.Update(entity);
        await Db.SaveChangesAsync(ct);
    }

    public virtual async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        Set.Remove(entity);
        await Db.SaveChangesAsync(ct);
    }

    public virtual async Task<T?> FindByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        await Set.FirstOrDefaultAsync(predicate, ct);

    public virtual Task<bool> ExistsByAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default) =>
        Set.AnyAsync(predicate, ct);

    protected async Task<PagedResponse<T>> GetPagedAsync(
        IQueryable<T> query,
        PaginationFilter filter,
        string defaultSort = "",
        CancellationToken ct = default)
    {
        var page = Math.Max(1, filter.Page);
        var pageSize = Math.Clamp(filter.PageSize, 1, 100);
        var sortBy = string.IsNullOrWhiteSpace(filter.SortBy) ? defaultSort : filter.SortBy;

        var total = await query.CountAsync(ct);

        var items = await query
            .ApplySort(sortBy)
            .ApplyPagination(page, pageSize)
            .ToListAsync(ct);

        return new PagedResponse<T>
        {
            Data = [.. items],
            PageNumber = page,
            PageSize = pageSize,
            TotalRecords = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize)
        };
    }
}
