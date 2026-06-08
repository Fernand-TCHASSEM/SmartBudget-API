using System;
using SmartBudget.Domain.Interfaces.Services;
using StackExchange.Redis;

namespace SmartBudget.Infrastructure.Services;

public class RateLimitService(IConnectionMultiplexer redis) : IRateLimitService
{
    // Sliding window using a sorted set: members are request timestamps,
    // score is the Unix timestamp in milliseconds.
    private static readonly LuaScript _script = LuaScript.Prepare("""
        local key = @key
        local now = tonumber(@now)
        local window = tonumber(@window)
        local limit = tonumber(@limit)
        local clearBefore = now - window * 1000

        redis.call('ZREMRANGEBYSCORE', key, '-inf', clearBefore)
        local count = redis.call('ZCARD', key)

        if count < limit then
            redis.call('ZADD', key, now, now)
            redis.call('PEXPIRE', key, window * 1000)
            return {1, limit - count - 1}
        end

        local oldest = tonumber(redis.call('ZRANGE', key, 0, 0, 'WITHSCORES')[2])
        local retryAfter = math.ceil((oldest + window * 1000 - now) / 1000)
        return {0, 0, retryAfter}
    """);

    public async Task<(bool Allowed, int Remaining, int RetryAfterSeconds)> CheckAsync(
        string key, int maxRequests, int windowSeconds
    )
    {
        var db = redis.GetDatabase();
        var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        var result = (RedisResult[]?) await db.ScriptEvaluateAsync(_script, new
        {
            key = (RedisKey)$"rate_limit:{key}",
            now = (RedisValue)now,
            window = (RedisValue)windowSeconds,
            limit = (RedisValue)maxRequests,
        });

        var allowed = (int)result![0] == 1;
        var remaining = allowed ? (int)result![1] : 0;
        var retryAfter = (!allowed && result.Length > 2) ? (int)result[2] : 0;

        return (allowed, remaining, retryAfter);
    }
}
