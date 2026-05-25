using Microsoft.Extensions.Configuration;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Interfaces.Services;

namespace SmartBudget.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHash,
    IConfiguration config
)
{
    /* public async Task<UserResponse?> GetCurrentUserAsync(string userId)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user is null) return null;

        return new UserResponse(
            Id: user.Id,
            Email: user.Email,
            FirstName: user.FirstName,
            LastName: user.LastName,
            Currency: user.Currency,
            MonthStartDay: user.MonthStartDay,
            CreatedAt: user.CreatedAt
        );
    } */

    public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
    {
        var refreshTokenExpiryDays = int.Parse(config["JwtSettings:RefreshTokenExpiryDays"]!);

        var user = await userRepository.AddAsync(new User
        {
            Email = request.Email,
            PasswordHash = passwordHash.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Currency = request.Currency
        });

        var refreshToken = await refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays)
        });

        return new RegisterResponse(
            AccessToken: tokenService.GenerateAccessToken(user),
            RefreshToken: refreshToken.Token,
            ExpriesIn: refreshTokenExpiryDays * 24 * 60 * 60,
            User: new UserResponse(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Currency: user.Currency,
                MonthStartDay: user.MonthStartDay,
                CreatedAt: user.CreatedAt
            )
        );
    }
}
