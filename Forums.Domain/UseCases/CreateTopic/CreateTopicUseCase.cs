using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Models;
using Forums.Domain.UseCases.GetForums;
using MediatR;

namespace Forums.Domain.UseCases.CreateTopic;

internal class CreateTopicUseCase : IRequestHandler<CreateTopicCommand, Topic>
{
    private readonly IIntentionManager _intentionManager;
    private readonly IIdentityProvider _identityProvider;
    private readonly IGetForumsStorage _getForumsStorage;
    private readonly ICreateTopicStorage _storage;

    public CreateTopicUseCase(
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        IGetForumsStorage getForumsStorage,
        ICreateTopicStorage storage)
    {
        _intentionManager = intentionManager;
        _identityProvider = identityProvider;
        _getForumsStorage = getForumsStorage;
        _storage = storage;
    }

    public async Task<Topic> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        var (forumId, title) = command;

        _intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await _getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

        var result = await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);

        return result;
    }
}