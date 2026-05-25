using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartBudget.Application.Services;

namespace SmartBudget.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<AuthService>();
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        return services;
    }
}
