using SmartBudget.Domain.Entities;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task RevokeAsync(string token, CancellationToken ct = default);
    Task RevokeAllByUserIdAsync(string userId, CancellationToken ct = default);
}
