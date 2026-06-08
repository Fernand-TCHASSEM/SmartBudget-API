using FluentValidation;
using SmartBudget.Application.DTOs.Auth;
using SmartBudget.Domain.Interfaces.Repositories;

namespace SmartBudget.Application.Validators.Auth;

public class LoginDtoValidator : AbstractValidator<LoginRequest>
{
    public LoginDtoValidator(IUserRepository userRepository)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255)
            .MustAsync(async (email, _) =>
            {
                return await userRepository.ExistsByAsync(u => u.Email == email, _);
            })
            .WithMessage("User with this email does not exist.");

        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
