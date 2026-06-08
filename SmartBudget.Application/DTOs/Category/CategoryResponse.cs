namespace SmartBudget.Application.DTOs.Category;

public record CategoryResponse(
    string Id,
    string? UserId,
    string Name,
    string Icon,
    string Color,
    bool IsDefault,
    bool IsIncome,
    int SortOrder,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DeletedAt
);
