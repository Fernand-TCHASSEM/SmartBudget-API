using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Interfaces.Services;
using SmartBudget.Infrastructure.Persistence;
using SmartBudget.Infrastructure.Repositories;
using SmartBudget.Infrastructure.Services;

namespace SmartBudget.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")!;
        services.AddSingleton<SoftDeleteInterceptor>();
        services.AddDbContext<SmartBudgetDbContext>((sp, options) =>
            options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
            .AddInterceptors(sp.GetRequiredService<SoftDeleteInterceptor>())
            /* .UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Action" },
                        new Genre { Name = "Adventure" },
                        new Genre { Name = "RPG" },
                        new Genre { Name = "Strategy" },
                        new Genre { Name = "Sports" }
                    );
                    context.SaveChanges();
                }
            }) */
        );

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        return services;
    }
}
