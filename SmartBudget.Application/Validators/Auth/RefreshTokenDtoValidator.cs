using FluentValidation;
using SmartBudget.Application.DTOs.Auth;

namespace SmartBudget.Application.Validators.Auth;

public class RefreshTokenDtoValidator : AbstractValidator<RefreshRequest>
{
    public RefreshTokenDtoValidator()
    {
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}
