using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.SignIn;

internal class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(c => c.Login)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY)
            .MaximumLength(20).WithErrorCode(ValidationErrorCode.TOO_LONG);
        RuleFor(c => c.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
    }
}