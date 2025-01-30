using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.SignOn;

internal class SignOnCommandValidator : AbstractValidator<SignOnCommand>
{
    public SignOnCommandValidator()
    {
        RuleFor(c => c.Login)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY)
            .MaximumLength(20).WithErrorCode(ValidationErrorCode.TOO_LONG);
        RuleFor(c => c.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
    }
}