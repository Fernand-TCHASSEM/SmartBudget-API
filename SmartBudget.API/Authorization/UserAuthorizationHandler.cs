using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.User;

namespace SmartBudget.API.Authorization;

public class UserAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, UserResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        UserResponse user)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (user.Id == userId)
            context.Succeed(requirement);
        else
            context.Fail();

        return Task.CompletedTask;
    }
}
