using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.Auth;

public record RegisterRequest (
    string Email,
    string Password,
    string FirstName,
    string? LastName = null,
    Currency Currency = Currency.CAD
);
