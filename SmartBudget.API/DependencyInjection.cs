using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using SmartBudget.API.Authorization;
using SmartBudget.API.Results;

namespace SmartBudget.API;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllers();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
                new UnprocessableEntityObjectResult(
                    new ValidationProblemDetails(context.ModelState))
                {
                    ContentTypes = { "application/problem+json" }
                };
        });
        services.AddFluentValidationAutoValidation(cfg =>
            cfg.OverrideDefaultResultFactoryWith<UnprocessableEntityResultFactory>());

        var secretKey = config["JwtSettings:SecretKey"];
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new InvalidOperationException("JwtSettings:SecretKey is not configured.");

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["JwtSettings:Issuer"],
                ValidAudience = config["JwtSettings:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        services.AddAuthorization();
        services.AddSingleton<IAuthorizationHandler, UserAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, CategoryAuthorizationHandler>();
        services.AddSingleton<IAuthorizationHandler, CategoryRuleAuthorizationHandler>();

        return services;
    }
}
