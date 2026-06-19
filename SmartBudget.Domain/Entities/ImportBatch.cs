using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class ImportBatch : ISoftDeletable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public required string FileName { get; set; }

    public required FileType FileType { get; set; }

    public string? BlobUrl { get; set; }

    public ImportStatus Status { get; set; } = ImportStatus.PENDING;

    public int TotalRows { get; set; } = 0;

    public int ImporteCount { get; set; } = 0;

    public int DuplicateCount { get; set; } = 0;

    public int ErrorCount { get; set; } = 0;

    public string? ErrorMesage {get; set;}

    public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public required string UserId { get; set; }

    public required string BankAccountId { get; set; }

    public User User { get; set; } = null!;

    public BankAccount BankAccount { get; set; } = null!;

    public ICollection<Transaction> Transactions { get; set; } = [];
}