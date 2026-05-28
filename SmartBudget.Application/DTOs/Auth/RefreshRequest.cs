namespace SmartBudget.Application.DTOs.Auth;

public record RefreshRequest (
    string Token,
    string RefreshToken
);
