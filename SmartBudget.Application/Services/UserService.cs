using Microsoft.Extensions.Logging;
using SmartBudget.Application.DTOs.User;
using SmartBudget.Domain.Entities;
using SmartBudget.Domain.Interfaces.Repositories;
using SmartBudget.Domain.Interfaces.Services;

namespace SmartBudget.Application.Services;

public class UserService(
    IUserRepository userRepository,
    IPasswordHasher passwordHash,
    ILogger<UserService> logger
)
{
    public async Task<UserResponse?> GetByIdAsync(string userId, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        return user is null ? null : MapToResponse(user);
    }

    public async Task<UserResponse?> UpdateAsync(string userId, UpdateUserRequest request, CancellationToken ct = default)
    {
        var user = await userRepository.GetByIdAsync(userId, ct);
        if (user is null)
        {
            logger.LogWarning("User not found: {UserId}", userId);
            return null;
        }

        user.FirstName     = request.FirstName;
        user.LastName      = request.LastName;
        user.MonthStartDay = request.MonthStartDay;
        if (request.Password is not null) user.PasswordHash = passwordHash.Hash(request.Password);
        user.Currency      = request.Currency;

        await userRepository.UpdateAsync(user, ct);
        return MapToResponse(user);
    }

    private static UserResponse MapToResponse(User u) => new(
        Id: u.Id,
        Email: u.Email,
        FirstName: u.FirstName,
        LastName: u.LastName,
        Currency: u.Currency,
        MonthStartDay: u.MonthStartDay,
        IsActive: u.IsActive,
        CreatedAt: u.CreatedAt,
        UpdatedAt: u.UpdatedAt,
        DeletedAt: u.DeletedAt
    );
}
