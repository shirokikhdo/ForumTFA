using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.CreateTopic;

internal class CreateTopicCommandValidator : AbstractValidator<CreateTopicCommand>
{
    public CreateTopicCommandValidator()
    {
        RuleFor(c => c.ForumId).NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
        RuleFor(c => c.Title).Cascade(CascadeMode.Stop)
            .NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY)
            .MaximumLength(100).WithErrorCode(ValidationErrorCode.TOO_LONG);
    }
}