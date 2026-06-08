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
            .MaximumLength(10)
            .When(c => c.Icon is not null);
        RuleFor(c => c.Color)
            .Length(7)
            .Matches("^#[0-9A-Fa-f]{6}$").WithMessage("Color must be a valid hex code.")
            .When(x => x.Color is not null);
        RuleFor(c => c.SortOrder)
            .GreaterThanOrEqualTo(0)
            .When(x => x.SortOrder.HasValue);
    }
}
