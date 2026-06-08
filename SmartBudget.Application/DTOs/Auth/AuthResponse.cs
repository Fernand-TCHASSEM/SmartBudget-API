using SmartBudget.Application.DTOs.User;

namespace SmartBudget.Application.DTOs.Auth;

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer",
    UserResponse? User = null
);
