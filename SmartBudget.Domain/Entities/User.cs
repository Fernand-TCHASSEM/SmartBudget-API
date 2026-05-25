using System.ComponentModel.DataAnnotations;
using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces;

namespace SmartBudget.Domain.Entities;

public class User : ISoftDeletable
{
    [Length(36, 36)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];

    [MaxLength(255)]
    [EmailAddress]
    public required string Email { get; set; }

    [MaxLength(512)]
    public required string PasswordHash { get; set; }

    [MaxLength(100)]
    public required string FirstName { get; set; }

    [MaxLength(100)]
    public string? LastName { get; set; } = null;

    public Currency Currency { get; set; } = Currency.CAD;

    [Range(1, 28)]
    public byte MonthStartDay { get; set; } = 1;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? DeletedAt { get; set; }
}