using FluentValidation;
using SmartBudget.Application.DTOs.Category;

namespace SmartBudget.Application.Validators.Category;

public class UpdateCategoryDtoValidator : AbstractValidator<UpdateCategoryRequest>
{
    public UpdateCategoryDtoValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(c => c.Icon)
            .NotEmpty()
            .MaximumLength(10);

        RuleFor(c => c.Color)
            .NotEmpty()
            .MaximumLength(7)
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex code (e.g. #3991BC).");
    }
}
