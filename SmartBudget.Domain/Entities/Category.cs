using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class Category : ISoftDeletable, IHasTimestamps
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required string Name { get; set; }

    public string Icon { get; set; } = "❓";

    public string Color { get; set; } = "#6B7280";

    public bool IsDefault { get; set; } = false;

    public bool IsIncome { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public string? UserId { get; set; }

    public User? User { get; set; }

    public ICollection<CategoryRule> CategoryRules { get; set; } = [];

    public ICollection<Transaction> Transactions { get; set; } = [];

    public ICollection<Budget> Budgets { get; set; } = [];
}
