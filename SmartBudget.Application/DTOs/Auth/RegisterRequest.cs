using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.Auth;

public record RegisterRequest (
    string Email,
    string FirstName,
    string Password,
    string? LastName = null,
    Currency Currency = Currency.CAD
);
