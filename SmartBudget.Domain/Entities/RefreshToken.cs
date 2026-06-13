namespace SmartBudget.Domain.Entities;

public class RefreshToken
{
    public string Id { get; init; } = Guid.NewGuid().ToString();

    public required string Token { get; set; }

    public required string UserId { get; set; }

    public DateTime ExpiresAt { get; set; }

    public bool IsRevoked { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
}
