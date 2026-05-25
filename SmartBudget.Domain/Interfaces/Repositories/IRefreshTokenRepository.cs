using SmartBudget.Domain.Entities;

namespace SmartBudget.Domain.Interfaces.Repositories;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeAsync(string token);
    Task RevokeAllByUserIdAsync(string userId);
}
