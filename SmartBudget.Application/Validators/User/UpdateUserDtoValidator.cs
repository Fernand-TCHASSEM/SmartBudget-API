using FluentValidation;
using SmartBudget.Application.DTOs.User;
using SmartBudget.Domain.Enums;

namespace SmartBudget.Application.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserDtoValidator()
    {
        RuleFor(u => u.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(u => u.MonthStartDay)
            .InclusiveBetween((byte)1, (byte)28);

        RuleFor(u => u.LastName)
            .MaximumLength(100);

        RuleFor(u => u.Password)
            .MinimumLength(8)
            .When(x => x.Password is not null);

        RuleFor(u => u.Currency).IsInEnum()
            .WithMessage($"Currency must be one of: {string.Join(", ", Enum.GetNames<Currency>())}.");
    }
}
