using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace SmartBudget.API.Authorization.Operation;

public static class BankAccountOperations
{
    public static readonly OperationAuthorizationRequirement Show = new() { Name = nameof(Show) };
    public static readonly OperationAuthorizationRequirement Update = new() { Name = nameof(Update) };
    public static readonly OperationAuthorizationRequirement Delete = new() { Name = nameof(Delete) };
}
