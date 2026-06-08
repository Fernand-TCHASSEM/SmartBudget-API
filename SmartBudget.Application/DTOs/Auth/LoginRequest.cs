using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.Auth;

public record LoginRequest (
    string Email,
    string Password
);
