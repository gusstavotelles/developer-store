using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        RuleFor(user => user.Email).SetValidator(new EmailValidator());

        RuleFor(user => user.Username)
            .NotEmpty()
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(user => user.Password).SetValidator(new PasswordValidator());

        RuleFor(user => user.Phone)
            .Matches(@"(^\+?[1-9]\d{1,14}$)|(^\(?\d{2}\)?\s?\d{4,5}-\d{4}$)")
            .WithMessage("Phone number must be in E.164 or local format (e.g. +5511987654321 or (11) 98765-4321).");

        RuleFor(user => user.Status)
            .NotEqual(UserStatus.Unknown)
            .WithMessage("User status cannot be Unknown.");

        RuleFor(user => user.Role)
            .NotEqual(UserRole.None)
            .WithMessage("User role cannot be None.");
    }
}
