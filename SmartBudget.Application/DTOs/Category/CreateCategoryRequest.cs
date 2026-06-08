namespace SmartBudget.Application.DTOs.Category;

public record CreateCategoryRequest (
    string Name,
    string? Icon,
    string? Color,
    bool IsIncome,
    int? SortOrder = 0
);
