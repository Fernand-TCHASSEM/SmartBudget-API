namespace SmartBudget.Application.DTOs.CategoryRule;

public record CreateCategoryRuleRequest (
    string Name,
    string Keyword,
    bool IsRegex = false,
    int Priority = 100
);
