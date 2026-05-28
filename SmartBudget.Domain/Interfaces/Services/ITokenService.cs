using System.Security.Claims;
using SmartBudget.Domain.Entities;

namespace SmartBudget.Domain.Interfaces.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);

    string GenerateRefreshToken();

    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}
