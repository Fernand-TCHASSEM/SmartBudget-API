namespace SmartBudget.Application.DTOs.Auth;

public record RegisterResponse(
    string AccessToken,
    string RefreshToken,
    int ExpriesIn,
    string TokenType = "Bearer",
    UserResponse? User = null
);
