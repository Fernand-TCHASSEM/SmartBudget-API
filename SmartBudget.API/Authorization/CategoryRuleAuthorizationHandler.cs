using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using SmartBudget.API.Authorization.Operation;
using SmartBudget.Application.DTOs.CategoryRule;

namespace SmartBudget.API.Authorization;

public class CategoryRuleAuthorizationHandler
    : AuthorizationHandler<OperationAuthorizationRequirement, CategoryRuleResponse>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OperationAuthorizationRequirement requirement,
        CategoryRuleResponse rule)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (requirement == CategoryRuleOperations.Delete && rule.UserId == userId)
            context.Succeed(requirement);

        return Task.CompletedTask;
    }
}
