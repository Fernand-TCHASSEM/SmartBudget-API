using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.User;

public record UserResponse(
    string Id,
    string Email,
    string FirstName,
    string? LastName,
    Currency Currency,
    byte MonthStartDay,
    DateTime CreatedAt
);
