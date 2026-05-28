using Microsoft.EntityFrameworkCore;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Infrastructure.Persistence;

namespace SmartBudget.Infrastructure.Repositories;

public class RefreshTokenRepository(SmartBudgetDbContext db) : Repository<RefreshToken>(db), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        await Set
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public async Task RevokeAsync(string token, CancellationToken ct = default)
    {
        var refreshToken = await this.FindByAsync(r => r.Token == token, ct);
        if (refreshToken is null) return;
        refreshToken.IsRevoked = true;
        await Db.SaveChangesAsync(ct);
    }

    public async Task RevokeAllByUserIdAsync(string userId, CancellationToken ct = default) =>
        await Set
            .Where(r => r.UserId == userId && !r.IsRevoked)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.IsRevoked, true), ct);
}
