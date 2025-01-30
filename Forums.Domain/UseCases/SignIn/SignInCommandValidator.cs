using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.SignIn;

internal class SignInCommandValidator : AbstractValidator<SignInCommand>
{
    public SignInCommandValidator()
    {
        RuleFor(c => c.Login).Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
        RuleFor(c => c.Password)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
    }
}