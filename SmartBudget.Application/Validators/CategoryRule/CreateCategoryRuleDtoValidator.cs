using FluentValidation;
using SmartBudget.Application.DTOs.CategoryRule;

namespace SmartBudget.Application.Validators.CategoryRule;

public class CreateCategoryRuleDtoValidator : AbstractValidator<CreateCategoryRuleRequest>
{
    public CreateCategoryRuleDtoValidator()
    {
        RuleFor(cr => cr.Name)
            .MaximumLength(150);
        RuleFor(cr => cr.Keyword)
            .NotEmpty()
            .MaximumLength(255);
    }
}
