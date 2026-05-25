using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Application.Services;

namespace SmartBudget.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthController(AuthService authService) : ControllerBase
    {
        /* [HttpGet]
        [Authorize]
        [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return Unauthorized();

            return await authService.GetCurrentUserAsync(userId) is UserResponse response
                ? Ok(response)
                : NotFound();
        } */

        [HttpPost]
        [ProducesResponseType<RegisterResponse>(StatusCodes.Status200OK)]
        [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CreateUser(RegisterRequest dto, CancellationToken ct)
        {
            return await authService.RegisterAsync(dto) is RegisterResponse response
                ? Ok(response)
                : Problem("An error occurred while creating the user.", statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
