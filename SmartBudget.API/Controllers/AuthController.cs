using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Application.Errors;
using SmartBudget.Application.Services;

namespace SmartBudget.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        /// <summary>
        /// Register a new user account.
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateUser(RegisterRequest dto, CancellationToken ct)
        {
            // CreatedAtAction(nameof(GetById), new { id = response.UserId }, response)
            return await authService.RegisterAsync(dto, ct) is AuthResponse response
                ? Created("", response)
                : Problem("Failed to create user.", statusCode: StatusCodes.Status500InternalServerError);
        }

        /// <summary>
        /// Log in to an existing user account.
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Login(LoginRequest dto, CancellationToken ct)
        {
            var (response, error) = await authService.LoginAsync(dto, ct);

            return error switch
            {
                null                         => Ok(response),
                AuthError.InvalidCredentials => Problem("Invalid email or password.", statusCode: 401),
                AuthError.AccountInactive    => Problem("Account is disabled.", statusCode: 403),
                _                            => Problem(statusCode: 500)
            };
        }

        /// <summary>
        /// Refresh an existing access token.
        /// </summary>
        [HttpPost("refresh")]
        [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RefreshToken(RefreshRequest dto, CancellationToken ct)
        {
            var (response, error) = await authService.RefreshAsync(dto, ct);

            return error switch
            {
                null                          => Ok(response),
                AuthError.InvalidToken or
                AuthError.InvalidRefreshToken => Problem("Invalid or expired token.", statusCode: 401),
                AuthError.AccountInactive     => Problem("Account is disabled.", statusCode: 403),
                _                             => Problem(statusCode: 500)
            };
        }

        /// <summary>
        /// Revoke the current refresh token.
        /// </summary>
        [HttpPost("revoke")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RevokeToken(RevokeRequest dto, CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId is null)
                return Problem("User ID not found in token.", statusCode: 401);

            var (response, error) = await authService.RevokeAsync(userId, dto, ct);

            return error switch
            {
                null                          => Ok(response),
                AuthError.InvalidRevokeToken  => Problem("Invalid or expired token.", statusCode: 401),
                AuthError.AccountInactive     => Problem("Account is disabled.", statusCode: 403),
                _                             => Problem(statusCode: 500)
            };
        }
    }
}
