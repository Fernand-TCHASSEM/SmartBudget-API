using Microsoft.Extensions.Logging;
using SmartBudget.Application.DTOs.BankAccount;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.Services;

public class BankAccountService(
    IBankAccountRepository bankAccountRepository,
    ILogger<BankAccountService> logger
)
{
    public async Task<PagedResponse<BankAccountResponse>> GetPagedAsync(string userId, BankAccountQuery query, CancellationToken ct = default)
    {
        var result = await bankAccountRepository.GetPagedForUserAsync(userId, query, query.AccountType, query.Currency, ct);

        return result.Map(MapToResponse);
    }

    public async Task<BankAccountResponse?> GetByIdAsync(string bankAccountId, CancellationToken ct = default)
    {
        var bankAccount = await bankAccountRepository.FindByAsync(ba => ba.Id == bankAccountId, ct);
        return bankAccount is null ? null : MapToResponse(bankAccount);
    }

    public async Task<BankAccountResponse> CreateAsync(string userId, CreateBankAccountRequest request, CancellationToken ct = default)
    {
        var bankAccount = await bankAccountRepository.AddAsync(new BankAccount
        {
            Name          = request.Name,
            BankName      = request.BankName,
            UserId        = userId,
            AccountType   = request.AccountType,
            Currency      = request.Currency
        }, ct);

        return MapToResponse(bankAccount);
    }

    public async Task<BankAccountResponse?> UpdateAsync(string bankAccountId, UpdateBankAccountRequest request, CancellationToken ct = default)
    {
        var bankAccount = await bankAccountRepository.FindByAsync(ba => ba.Id == bankAccountId, ct);
        if (bankAccount is null)
        {
            logger.LogWarning("Bank account not found: {BankAccountId}", bankAccountId);
            return null;
        }

        bankAccount.Name        = request.Name;
        bankAccount.BankName    = request.BankName;
        bankAccount.AccountType = request.AccountType;
        bankAccount.Currency    = request.Currency;

        await bankAccountRepository.UpdateAsync(bankAccount, ct);
        return MapToResponse(bankAccount);
    }

    public async Task<bool> DeleteAsync(string bankAccountId, CancellationToken ct = default)
    {
        var bankAccount = await bankAccountRepository.FindByAsync(ba => ba.Id == bankAccountId, ct);
        if (bankAccount is null)
        {
            logger.LogWarning("Bank account not found: {BankAccountId}", bankAccountId);
            return false;
        }

        await bankAccountRepository.DeleteAsync(bankAccount, ct);
        return true;
    }

    private static BankAccountResponse MapToResponse(BankAccount ba) => new(
        Id:          ba.Id,
        UserId:      ba.UserId,
        Name:        ba.Name,
        BankName:    ba.BankName,
        AccountType: ba.AccountType,
        Currency:    ba.Currency,
        CreatedAt:   ba.CreatedAt,
        UpdatedAt:   ba.UpdatedAt,
        DeletedAt:   ba.DeletedAt
    );
}
