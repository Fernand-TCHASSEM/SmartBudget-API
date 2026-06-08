namespace SmartBudget.Application.DTOs.Category;

public record UpdateCategoryRequest (
    string Name,
    string Icon,
    string Color,
    bool IsIncome,
    int SortOrder
);
