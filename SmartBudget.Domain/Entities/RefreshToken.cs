using System.ComponentModel.DataAnnotations;

namespace SmartBudget.Domain.Entities;

public class RefreshToken
{
    [Length(36, 36)]
    public string Id { get; init; } = Guid.NewGuid().ToString();

    [MaxLength(512)]
    public required string Token { get; set; }

    public required DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;

    public required string UserId { get; set; }
}
