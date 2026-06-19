using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.BankAccount;

public record BankAccountResponse(
    string Id,
    string UserId,
    string Name,
    string BankName,
    AccountType AccountType,
    Currency Currency,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    DateTime? DeletedAt
);
