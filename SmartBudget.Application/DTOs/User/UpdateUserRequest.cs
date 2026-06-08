using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.User;

public record UpdateUserRequest (
    string FirstName,
    byte MonthStartDay,
    string? Password,
    string? LastName = null,
    Currency Currency = Currency.CAD
);
