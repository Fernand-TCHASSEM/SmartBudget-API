using System;

namespace SmartBudget.Domain.Interfaces.Services;

public interface IRateLimitService
{
    Task<(bool Allowed, int Remaining, int RetryAfterSeconds)> CheckAsync(
        string key,
        int maxRequests,
        int windowSeconds);
}
