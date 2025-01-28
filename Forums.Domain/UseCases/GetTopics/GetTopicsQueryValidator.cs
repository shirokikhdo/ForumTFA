using FluentValidation;
using Forums.Domain.Exceptions;

namespace Forums.Domain.UseCases.GetTopics;

internal class GetTopicsQueryValidator : AbstractValidator<GetTopicsQuery>
{
    public GetTopicsQueryValidator()
    {
        RuleFor(q => q.ForumId).NotEmpty().WithErrorCode(ValidationErrorCode.EMPTY);
        RuleFor(q => q.Skip).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.INVALID);
        RuleFor(q => q.Take).GreaterThanOrEqualTo(0).WithErrorCode(ValidationErrorCode.INVALID);
    }
}