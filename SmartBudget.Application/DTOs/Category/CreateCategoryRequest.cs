namespace SmartBudget.Application.DTOs.Category;

public record CreateCategoryRequest (
    string Name,
    bool IsIncome,
    string Icon =  "❓",
    string Color = "#6B7280",
    int SortOrder = 0
);
