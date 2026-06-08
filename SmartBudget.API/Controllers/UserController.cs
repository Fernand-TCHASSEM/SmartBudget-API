using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.User;
using SmartBudget.Application.Services;

namespace SmartBudget.API.Controllers;

[Route("api/users")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class UserController(
    UserService userService,
    IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>Get a user's data by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Show(string id, CancellationToken ct)
    {
        var userToShow = await userService.GetByIdAsync(id, ct);
        if (userToShow is null) return NotFound();

        var auth = await authorizationService.AuthorizeAsync(User, userToShow, UserOperations.View);
        if (!auth.Succeeded) return Forbid();

        return Ok(userToShow);
    }

    /// <summary>Update a user'data by ID.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(string id, UpdateUserRequest dto, CancellationToken ct)
    {
        var userToUpdate = await userService.GetByIdAsync(id, ct);
        if (userToUpdate is null || !userToUpdate.IsActive) return NotFound();

        var auth = await authorizationService.AuthorizeAsync(User, userToUpdate, UserOperations.Update);
        if (!auth.Succeeded) return Forbid();

        return Ok(await userService.UpdateAsync(id, dto, ct));
    }
}
