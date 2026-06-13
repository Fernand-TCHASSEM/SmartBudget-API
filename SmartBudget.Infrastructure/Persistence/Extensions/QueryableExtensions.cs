using System.Linq.Dynamic.Core;
using System.Reflection;

namespace SmartBudget.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int page, int pageSize) =>
        query.Skip((page - 1) * pageSize).Take(pageSize);

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string? sortBy) where T : class
    {
        if (string.IsNullOrWhiteSpace(sortBy)) return query;

        var allowed = typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var clauses = sortBy
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(part =>
            {
                var tokens = part.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length == 0 || !allowed.Contains(tokens[0])) return null;
                var direction = tokens.Length > 1 && tokens[1].Equals("desc", StringComparison.OrdinalIgnoreCase)
                    ? "descending" : "ascending";
                return $"{tokens[0]} {direction}";
            })
            .OfType<string>()
            .ToList();

        return clauses.Count > 0 ? query.OrderBy(string.Join(", ", clauses)) : query;
    }
}
