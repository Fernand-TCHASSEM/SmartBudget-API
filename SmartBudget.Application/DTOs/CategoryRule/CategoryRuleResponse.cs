using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.CategoryRule;

public record CategoryRuleResponse(
    string Id,
    string? UserId,
    string CategoryId,
    string? Name,
    string Keyword,
    bool IsRegex,
    int Priority,
    RuleSource Source,
    DateTime CreatedAt,
    DateTime? DeletedAt
);
