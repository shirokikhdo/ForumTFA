using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.CreateForum;

internal class CreateForumCommandValidator : AbstractValidator<CreateForumCommand>
{
    public CreateForumCommandValidator()
    {
        RuleFor(c => c.Title)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY)
            .MaximumLength(50).WithErrorCode(ValidationErrorCode.TOO_LONG);
    }
}