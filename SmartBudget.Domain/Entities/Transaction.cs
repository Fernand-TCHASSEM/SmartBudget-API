using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class Transaction : ISoftDeletable, IHasTimestamps
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string RawLabel { get; set; }

    public required string CleanLabel { get; set; }

    public required decimal Amount { get; set; }

    public required TransactionType Type { get; set; }

    public bool IsCategoryManual { get; set; } = false;

    public string? Note { get; set; }

    public bool IsExcluded { get; set; } = false;

    public required string Hash { get; set; }

    public required DateOnly TransactionDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public required string UserId { get; set; }

    public required string BankAccountId { get; set; }

    public string? ImportBatchId { get; set; }

    public string? CategoryId { get; set; }

    public User User { get; set; } = null!;

    public BankAccount BankAccount { get; set; } = null!;

    public ImportBatch? ImportBatch { get; set; }

    public Category? Category { get; set; }
}