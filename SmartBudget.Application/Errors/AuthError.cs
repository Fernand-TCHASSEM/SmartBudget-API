namespace SmartBudget.Application.Errors;

public enum AuthError
{
    InvalidCredentials,
    AccountInactive,
    InvalidToken,
    InvalidRefreshToken,
    InvalidRevokeToken
}
