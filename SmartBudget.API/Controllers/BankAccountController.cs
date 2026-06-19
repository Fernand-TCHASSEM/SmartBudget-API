using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.BankAccount;
using SmartBudget.Application.Services;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.API.Controllers;

[Route("api/bank-accounts")]
[ApiController]
[Authorize]
[Produces("application/json")]
public class BankAccountController(
    BankAccountService bankAccountService,
    IAuthorizationService authorizationService) : ControllerBase
{
    /// <summary>List bank accounts visible to the current user (own) with pagination, search and filters.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResponse<BankAccountResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Index([FromQuery] BankAccountQuery query, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        return Ok(await bankAccountService.GetPagedAsync(userId, query, ct));
    }

    /// <summary>Get a single bank account by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType<BankAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Show(string id, CancellationToken ct)
    {
        var bankAccount = await bankAccountService.GetByIdAsync(id, ct);
        if (bankAccount is null) return Problem("Bank account not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, bankAccount, BankAccountOperations.Show);
        if (!auth.Succeeded) return Problem("You do not have permission to access this bank account.", statusCode: 403);

        return Ok(bankAccount);
    }

    /// <summary>Create a new user-defined bank account.</summary>
    [HttpPost]
    [ProducesResponseType<BankAccountResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Store(CreateBankAccountRequest dto, CancellationToken ct)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var response = await bankAccountService.CreateAsync(userId, dto, ct);
        return CreatedAtAction(nameof(Show), new { id = response.Id }, response);
    }

    /// <summary>Update a user-defined bank account.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType<BankAccountResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string id, UpdateBankAccountRequest dto, CancellationToken ct)
    {
        var bankAccount = await bankAccountService.GetByIdAsync(id, ct);
        if (bankAccount is null) return Problem("Bank account not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, bankAccount, BankAccountOperations.Update);
        if (!auth.Succeeded) return Problem("You do not have permission to update this bank account.", statusCode: 403);

        return Ok(await bankAccountService.UpdateAsync(id, dto, ct));
    }

    /// <summary>Soft-delete a user-defined bank account.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Destroy(string id, CancellationToken ct)
    {
        var bankAccount = await bankAccountService.GetByIdAsync(id, ct);
        if (bankAccount is null) return Problem("Bank account not found.", statusCode: 404);

        var auth = await authorizationService.AuthorizeAsync(User, bankAccount, BankAccountOperations.Delete);
        if (!auth.Succeeded) return Problem("You do not have permission to delete this bank account.", statusCode: 403);

        await bankAccountService.DeleteAsync(id, ct);
        return NoContent();
    }
}
