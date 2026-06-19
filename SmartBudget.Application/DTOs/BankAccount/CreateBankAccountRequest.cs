using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.DTOs.BankAccount;

public record CreateBankAccountRequest (
    string Name,
    string BankName,
    AccountType AccountType,
    Currency Currency = Currency.CAD
);
