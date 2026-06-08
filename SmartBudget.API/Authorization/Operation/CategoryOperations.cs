using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SmartBudget.API.Authorization.Operation;

public static class CategoryOperations
{
    public static readonly OperationAuthorizationRequirement View   = new() { Name = nameof(View) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
}
