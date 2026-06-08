using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Application.DTOs.User;
using SmartBudget.Application.Errors;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Interfaces.Services;

namespace SmartBudget.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    ITokenService tokenService,
    IPasswordHasher passwordHash,
    IConfiguration config,
    ILogger<AuthService> logger
)
{
    private int RefreshTokenExpiryDays => int.Parse(config["JwtSettings:RefreshTokenExpiryDays"]!);

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.AddAsync(new User
        {
            Email = request.Email,
            PasswordHash = passwordHash.Hash(request.Password),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Currency = request.Currency
        }, ct);

        var refreshToken = await refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
        }, ct);

        return GenerateAuthResponse(user, refreshToken);
    }

    public async Task<(AuthResponse?, AuthError?)> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await userRepository.FindByAsync(u => u.Email == request.Email, ct);

        if (user is null || !passwordHash.Verify(request.Password, user.PasswordHash))
        {
            logger.LogWarning("Failed login attempt for email {Email}", request.Email);
            return (null, AuthError.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            logger.LogWarning("Login attempt on inactive account {UserId}", user.Id);
            return (null, AuthError.AccountInactive);
        }

        var refreshToken = await refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.Id,
            Token = tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
        }, ct);

        logger.LogInformation("User {UserId} logged in successfully", user.Id);
        return (GenerateAuthResponse(user, refreshToken), null);
    }

    public async Task<(AuthResponse?, AuthError?)> RefreshAsync(RefreshRequest request, CancellationToken ct)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.Token);
        var userId = principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId is null)
            return (null, AuthError.InvalidToken);

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        if (refreshToken is null || refreshToken.UserId != userId || refreshToken.ExpiresAt <= DateTime.UtcNow || refreshToken.IsRevoked)
        {
            logger.LogWarning("Invalid refresh token attempt for user {UserId}", userId);
            return (null, AuthError.InvalidRefreshToken);
        }

        if (!refreshToken.User.IsActive)
        {
            logger.LogWarning("Login attempt on inactive account {UserId}", userId);
            return (null, AuthError.AccountInactive);
        }

        await refreshTokenRepository.RevokeAsync(request.RefreshToken, ct);

        var newRefreshToken = await refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = userId,
            Token = tokenService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpiryDays)
        }, ct);

        return (GenerateAuthResponse(refreshToken.User, newRefreshToken), null);
    }

    public async Task<(AuthResponse?, AuthError?)> RevokeAsync(string userId, RevokeRequest request, CancellationToken ct)
    {

        var refreshToken = await refreshTokenRepository.GetByTokenAsync(request.RefreshToken, ct);
        if (refreshToken is null || refreshToken.UserId != userId || refreshToken.ExpiresAt <= DateTime.UtcNow || refreshToken.IsRevoked)
        {
            logger.LogWarning("Invalid refresh token attempt for user {UserId}", userId);
            return (null, AuthError.InvalidRevokeToken);
        }

        if (!refreshToken.User.IsActive)
        {
            logger.LogWarning("Login attempt on inactive account {UserId}", userId);
            return (null, AuthError.AccountInactive);
        }

        await refreshTokenRepository.RevokeAsync(request.RefreshToken, ct);

        return (null, null);
    }

    private AuthResponse GenerateAuthResponse(User user, RefreshToken refreshToken)
    {
        var accessTokenExpirySeconds = (int)(Convert.ToDouble(config["JwtSettings:AccessTokenExpiryMinutes"]) * 60);
        return new AuthResponse(
            AccessToken: tokenService.GenerateAccessToken(user),
            RefreshToken: refreshToken.Token,
            ExpiresIn: accessTokenExpirySeconds,
            User: new UserResponse(
                Id: user.Id,
                Email: user.Email,
                FirstName: user.FirstName,
                LastName: user.LastName,
                Currency: user.Currency,
                MonthStartDay: user.MonthStartDay,
                IsActive: user.IsActive,
                CreatedAt: user.CreatedAt,
                UpdatedAt: user.UpdatedAt,
                DeletedAt: user.DeletedAt
            )
        );
    }
}
