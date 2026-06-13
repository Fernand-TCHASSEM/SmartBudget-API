using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.DTOs.Category;

public class CategoryQuery : PaginationFilter
{
    public bool? IsIncome { get; set; }
    public bool? IsDefault { get; set; }
}
