using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SmartBudget.API.Authorization.Operation;

public static class CategoryRuleOperations
{
    public static readonly OperationAuthorizationRequirement Index = new() { Name = nameof(Index) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
}
