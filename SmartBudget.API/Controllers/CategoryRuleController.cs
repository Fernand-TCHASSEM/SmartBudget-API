using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.Category;
using SmartBudget.Application.DTOs.CategoryRule;
using SmartBudget.Application.Services;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.API.Controllers;

[Route("api/categories")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class CategoryRuleController(
    CategoryService categoryService,
    CategoryRuleService categoryRuleService,
    IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>List category rules visible to the current user (own + system defaults) with pagination, search and filters.</summary>
    [HttpGet("{categoryId}/rules")]
    [ProducesResponseType<PagedResponse<CategoryRuleResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Index(string categoryId, [FromQuery] PaginationFilter query, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(categoryId, ct);
        if (category is null) return Problem("Category not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, category, CategoryOperations.Show);
        if (!auth.Succeeded) return Problem("You do not have permission to access this category.", statusCode: 403);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await categoryRuleService.GetPagedAsync(userId, categoryId, query, ct));
    }

    /// <summary>Create a new rule on a category.</summary>
    [HttpPost("{categoryId}/rules")]
    [ProducesResponseType<CategoryRuleResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Store(string categoryId, CreateCategoryRuleRequest dto, CancellationToken ct)
    {
        var category = await categoryService.GetByIdAsync(categoryId, ct);
        if (category is null) return Problem("Category not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, category, CategoryOperations.Show);
        if (!auth.Succeeded) return Problem("You do not have permission to access this category.", statusCode: 403);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await categoryRuleService.CreateAsync(userId, categoryId, dto, ct);
        return CreatedAtAction(nameof(Index), new { categoryId }, response);
    }

    /// <summary>Soft-delete a rule.</summary>
    [HttpDelete("rules/{ruleId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Destroy(string ruleId, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var rule = await categoryRuleService.GetByIdAsync(ruleId, ct);
        if (rule is null) return Problem("Rule not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, rule, CategoryRuleOperations.Delete);
        if (!auth.Succeeded) return Problem("You do not have permission to delete this rule.", statusCode: 403);

        var deleted = await categoryRuleService.DeleteAsync(ruleId, ct);
        if (!deleted) return Problem("Rule not found.", statusCode: 404);
        return NoContent();
    }
}
