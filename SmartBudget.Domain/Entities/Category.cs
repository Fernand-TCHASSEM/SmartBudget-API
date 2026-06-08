using System.ComponentModel.DataAnnotations;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class Category : ISoftDeletable, IHasTimestamps
{
    [Length(36, 36)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [MaxLength(100)]
    public required string Name { get; set; }

    [MaxLength(10)]
    public required string Icon { get; set; } = "❓";

    [Length(7, 7)]
    public required string Color { get; set; } = "#6B7280";

    public bool IsDefault { get; set; } = false;

    public bool IsIncome { get; set; } = false;

    public int SortOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public User? User { get; set; } = null;

    public string? UserId { get; set; } = null;
}
