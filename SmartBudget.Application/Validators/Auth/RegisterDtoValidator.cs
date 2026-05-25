using FluentValidation;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Domain.Enums;
using SmartBudget.Domain.Interfaces.Repositories;

namespace SmartBudget.Application.Validators.Auth;

public class RegisterDtoValidator : AbstractValidator<RegisterRequest>
{
    public RegisterDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .MustAsync(async (email, _) =>
            {
                var exists = await userRepository.ExistsByEmailAsync(email);
                return !exists;
            })
            .WithMessage("Email is already in use.");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
        RuleFor(x=> x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).MaximumLength(100);
        RuleFor(x => x.Currency).IsInEnum()
            .WithMessage($"Currency must be one of: {string.Join(", ", Enum.GetNames<Currency>())}.");
    }
}
