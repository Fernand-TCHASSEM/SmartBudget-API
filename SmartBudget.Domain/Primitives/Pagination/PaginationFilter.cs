namespace SmartBudget.Domain.Primitives.Pagination;

public class PaginationFilter
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? Search { get; set; }
}
