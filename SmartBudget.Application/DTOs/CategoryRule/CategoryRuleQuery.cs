using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.DTOs.CategoryRule;

public class CategoryRuleQuery : PaginationFilter
{
    public bool? IsRegex { get; set; }
}
