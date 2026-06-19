using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.BankAccount;

public record UpdateBankAccountRequest (
    string Name,
    string BankName,
    AccountType AccountType,
    Currency Currency
);
