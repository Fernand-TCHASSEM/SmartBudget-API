using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.Category;

namespace SmartBudget.API.Authorization;

public class CategoryAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, CategoryResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        CategoryResponse category)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (requirement == CategoryOperations.Show)
        {
            if (category.IsDefault || category.UserId == userId)
                context.Succeed(requirement);

            return Task.CompletedTask;
        }

        // Update and Delete: system categories are immutable
        if (category.IsDefault)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        if (category.UserId == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
