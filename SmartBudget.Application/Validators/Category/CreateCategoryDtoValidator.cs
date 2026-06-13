using FluentValidation;
using SmartBudget.Application.DTOs.Category;

namespace SmartBudget.Application.Validators.Category;

public class CreateCategoryDtoValidator : AbstractValidator<CreateCategoryRequest>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(c => c.Icon)
            .MaximumLength(10);
        RuleFor(c => c.Color)
            .Length(7)
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex code.");
        RuleFor(c => c.SortOrder)
            .GreaterThanOrEqualTo(0);
    }
}
