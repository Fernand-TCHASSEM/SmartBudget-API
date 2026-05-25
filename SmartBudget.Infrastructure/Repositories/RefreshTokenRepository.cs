using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class RefreshTokenRepository(SmartBudgetDbContext db) : IRefreshTokenRepository
{
    public async Task<RefreshToken> AddAsync(RefreshToken token)
    {
        db.RefreshTokens.Add(token);
        await db.SaveChangesAsync();
        return token;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token);

    public async Task RevokeAsync(string token)
    {
        var refreshToken = await db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token);
        if (refreshToken is null) return;
        refreshToken.IsRevoked = true;
        await db.SaveChangesAsync();
    }

    public async Task RevokeAllByUserIdAsync(string userId)
    {
        await db.RefreshTokens
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true));
    }
}
