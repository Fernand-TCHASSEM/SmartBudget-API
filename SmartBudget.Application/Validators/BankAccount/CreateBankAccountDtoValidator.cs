using FluentValidation;
using SmartBudget.Application.DTOs.BankAccount;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.Validators.BankAccount;

public class CreateBankAccountDtoValidator : AbstractValidator<CreateBankAccountRequest>
{
    public CreateBankAccountDtoValidator()
    {
        RuleFor(ba => ba.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(ba => ba.BankName)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(ba => ba.Currency)
            .IsInEnum()
            .WithMessage($"Currency must be one of: {string.Join(", ", Enum.GetNames<Currency>())}.");

        RuleFor(ba => ba.AccountType)
            .IsInEnum()
            .WithMessage($"Account type must be one of: {string.Join(", ", Enum.GetNames<AccountType>())}.");
    }
}
