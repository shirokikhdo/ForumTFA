using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Exceptions;
using Topic = Forums.Domain.Models.Topic;

namespace Forums.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IIntentionManager _intentionManager;
    private readonly IIdentityProvider _identityProvider;
    private readonly ICreateTopicStorage _storage;

    public CreateTopicUseCase(
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        ICreateTopicStorage storage)
    {
        _intentionManager = intentionManager;
        _identityProvider = identityProvider;
        _storage = storage;
    }

    public async Task<Topic> Execute(
        Guid forumId,
        string title,
        CancellationToken cancellationToken)
    {
        _intentionManager.ThrowIfForbidden(TopicIntention.Create);
        var forumExists = await _storage.ForumExists(forumId, cancellationToken);

        if (!forumExists)
            throw new ForumNotFoundException(forumId);

        var result = await _storage.CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);

        return result;
    }
}