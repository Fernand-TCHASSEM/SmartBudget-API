using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class CategoryRule : ISoftDeletable
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public string? Name { get; set; }

    public required string Keyword { get; set; }

    public bool IsRegex { get; set; } = false;

    public int Priority { get; set; } = 100;

    public RuleSource Source { get; set; } = RuleSource.Manual;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public string? UserId { get; set; }

    public required string CategoryId { get; set; }

    public User? User { get; set; }

    public Category Category { get; set; } = null!;
}
