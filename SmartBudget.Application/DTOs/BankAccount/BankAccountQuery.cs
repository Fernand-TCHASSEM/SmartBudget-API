using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Primitives.Pagination;

namespace SmartBudget.Application.DTOs.BankAccount;

public class BankAccountQuery : PaginationFilter
{
    public AccountType? AccountType { get; set; }
    public Currency? Currency { get; set; }
}
