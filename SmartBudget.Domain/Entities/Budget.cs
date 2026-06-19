using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class Budget : ISoftDeletable, IHasTimestamps
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required int Year { get; set; }

    public byte Month { get; set; } = 1;

    public required decimal LimitAmount { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public required string UserId { get; set; }

    public required string CategoryId { get; set; }

    public User User { get; set; } = null!;

    public Category Category { get; set; } = null!;
}
