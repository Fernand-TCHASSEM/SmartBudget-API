using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.BankAccount;

namespace SmartBudget.API.Authorization;

public class BankAccountAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, BankAccountResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        BankAccountResponse bankAccount)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (bankAccount.UserId == userId)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}
