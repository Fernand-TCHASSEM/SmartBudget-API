using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBudget.Domain.Interfaces;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Interfaces.Services;
using SmartBudget.Infrastructure.Persistence;
using SmartBudget.Infrastructure.Repositories;
using SmartBudget.Infrastructure.Seeders;
using SmartBudget.Infrastructure.Services;
using StackExchange.Redis;

namespace SmartBudget.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")!;
        services.AddSingleton<HasTimestampsInterceptor>();
        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddDbContext<SmartBudgetDbContext>((sp, options) =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
            .AddInterceptors([
                sp.GetRequiredService<HasTimestampsInterceptor>(),
                sp.GetRequiredService<SoftDeleteInterceptor>(),
            ])
        );

        // Redis
        var redisConnectionString = config["Redis:ConnectionString"]
            ?? throw new InvalidOperationException("Redis:ConnectionString is not configured.");
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnectionString));
        services.AddSingleton<IRateLimitService, RateLimitService>();

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IDataSeeder, CategorySeeder>();
        return services;
    }
}
