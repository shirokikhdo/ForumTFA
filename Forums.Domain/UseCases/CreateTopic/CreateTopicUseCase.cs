using FluentValidation;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Exceptions;
using Forums.Domain.UseCases.GetForums;
using Topic = Forums.Domain.Models.Topic;

namespace Forums.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IValidator<CreateTopicCommand> _validator;
    private readonly IIntentionManager _intentionManager;
    private readonly IIdentityProvider _identityProvider;
    private readonly IGetForumsStorage _getForumsStorage;
    private readonly ICreateTopicStorage _storage;

    public CreateTopicUseCase(
        IValidator<CreateTopicCommand> validator,
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        IGetForumsStorage getForumsStorage,
        ICreateTopicStorage storage)
    {
        _validator = validator;
        _intentionManager = intentionManager;
        _identityProvider = identityProvider;
        _getForumsStorage = getForumsStorage;
        _storage = storage;
    }

    public async Task<Topic> Execute(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);
        
        var (forumId, title) = command;

        _intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await _getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

        var result = await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);

        return result;
    }
}