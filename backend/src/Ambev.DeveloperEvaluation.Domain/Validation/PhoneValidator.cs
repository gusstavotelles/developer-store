using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class PhoneValidator : AbstractValidator<string>
{
    public PhoneValidator()
    {
        RuleFor(phone => phone)
            .NotEmpty().WithMessage("The phone cannot be empty.")
            .Matches(@"(^\+?[1-9]\d{1,14}$)|(^\(?\d{2}\)?\s?\d{4,5}-\d{4}$)").WithMessage("The phone format is not valid.");
    }
}
