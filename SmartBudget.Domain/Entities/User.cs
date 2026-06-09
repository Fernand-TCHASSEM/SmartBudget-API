using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class User : ISoftDeletable, IHasTimestamps
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required string Email { get; set; }

    public required string PasswordHash { get; set; }

    public required string FirstName { get; set; }

    public string? LastName { get; set; }

    public Currency Currency { get; set; } = Currency.CAD;

    public byte MonthStartDay { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    public ICollection<Category> Categories { get; set; } = [];

    public ICollection<CategoryRule> CategoryRules { get; set; } = [];
}
