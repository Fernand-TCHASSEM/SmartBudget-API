using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.Category;
using SmartBudget.Application.Services;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.API.Controllers;

[Route("api/categories")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CategoryController(
    CategoryService categoryService,
    IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>List categories visible to the current user (own + system defaults) with pagination, search and filters.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResponse<CategoryResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Index([FromQuery] CategoryQuery query, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await categoryService.GetPagedAsync(userId, query, ct));
    }

    /// <summary>Get a single category by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Show(string id, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(id, ct);
        if (category is null) return Problem("Category not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, category, CategoryOperations.Show);
        if (!auth.Succeeded) return Problem("You do not have permission to access this category.", statusCode: 403);

        return Ok(category);
    }

    /// <summary>Create a new user-defined category.</summary>
    [HttpPost]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Store(CreateCategoryRequest dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await categoryService.CreateAsync(userId, dto, ct);
        return CreatedAtAction(nameof(Show), new { id = response.Id }, response);
    }

    /// <summary>Update a user-defined category.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType<CategoryResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, UpdateCategoryRequest dto, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(id, ct);
        if (category is null) return Problem("Category not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, category, CategoryOperations.Update);
        if (!auth.Succeeded) return Problem("You do not have permission to update this category.", statusCode: 403);

        return Ok(await categoryService.UpdateAsync(id, dto, ct));
    }

    /// <summary>Soft-delete a user-defined category.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Destroy(string id, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(id, ct);
        if (category is null) return Problem("Category not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, category, CategoryOperations.Delete);
        if (!auth.Succeeded) return Problem("You do not have permission to delete this category.", statusCode: 403);

        await categoryService.DeleteAsync(id, ct);
        return NoContent();
    }
}
