using SmartBudget.Domain.Interfaces.Services;

namespace SmartBudget.API.Middlewares;

public class RateLimitMiddleware(
    RequestDelegate next,
    IRateLimitService rateLimitService,
    IConfiguration config)
{
    private readonly bool _enabled = config.GetValue<bool>("RateLimitSettings:Enabled");
    private readonly int _globalMax = config.GetValue<int>("RateLimitSettings:GlobalMaxRequests");
    private readonly int _globalWindow = config.GetValue<int>("RateLimitSettings:GlobalWindowSeconds");
    private readonly int _authMax = config.GetValue<int>("RateLimitSettings:AuthMaxRequests");
    private readonly int _authWindow = config.GetValue<int>("RateLimitSettings:AuthWindowSeconds");

    public async Task InvokeAsync(HttpContext ctx)
    {
        if (!_enabled) { await next(ctx); return; }

        var ip = ctx.Connection.RemoteIpAddress?.ToString() ?? "unknow";
        var isAuthEndpoint = ctx.Request.Path.StartsWithSegments("/api/auth");

        var (maxRequests, windowSeconds) = isAuthEndpoint
        ? (_authMax, _authWindow)
        : (_globalMax, _globalWindow);

        // Authenticated users get a per-user key; anonymous requests use IP.
        var userId = ctx.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        var key = userId is not null ? $"user:{userId}" : $"ip:{ip}";
        if (isAuthEndpoint) key = $"auth:{ip}";  // always IP for auth endpoints

        var (allowed, remaining, retryAfter) = await rateLimitService.CheckAsync(
            key, maxRequests, windowSeconds
        );

        ctx.Response.Headers["X-RateLimit-Limit"] = maxRequests.ToString();
        ctx.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();

        if (!allowed)
        {
            ctx.Response.Headers["Retry-After"] = retryAfter.ToString();
            ctx.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await ctx.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc6585#section-4",
                title = "Too Many Requests",
                status = 429,
                retryAfterSeconds = retryAfter
            });
            return;
        }

        await next(ctx);
    }
}
