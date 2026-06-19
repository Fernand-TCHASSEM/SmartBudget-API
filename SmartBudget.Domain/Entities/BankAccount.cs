using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class BankAccount : ISoftDeletable, IHasTimestamps
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public required string BankName { get; set; }

    public AccountType AccountType { get; set; } = AccountType.CHEQUING;

    public Currency Currency { get; set; } = Currency.CAD;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public required string UserId { get; set; }

    public User User { get; set; } = null!;

    public ICollection<ImportBatch> ImportBatches { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];
}