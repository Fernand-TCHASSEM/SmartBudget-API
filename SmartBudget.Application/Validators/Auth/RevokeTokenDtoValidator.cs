using FluentValidation;
using SmartBudget.Application.DTOs.Auth;

namespace SmartBudget.Application.Validators.Auth;

public class RevokeTokenDtoValidator : AbstractValidator<RevokeRequest>
{
    public RevokeTokenDtoValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}
