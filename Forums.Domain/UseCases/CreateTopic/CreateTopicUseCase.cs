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
    private readonly IUnitOfWork _unitOfWork;

    public CreateTopicUseCase(
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        IGetForumsStorage getForumsStorage,
        IUnitOfWork unitOfWork)
    {
        _intentionManager = intentionManager;
        _identityProvider = identityProvider;
        _getForumsStorage = getForumsStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<Topic> Handle(CreateTopicCommand command, CancellationToken cancellationToken)
    {
        var (forumId, title) = command;

        _intentionManager.ThrowIfForbidden(TopicIntention.Create);

        await _getForumsStorage.ThrowIfForumNotFound(forumId, cancellationToken);

        await using var scope = await _unitOfWork.StartScope(cancellationToken);
        var result = await scope.GetStorage<ICreateTopicStorage>()
            .CreateTopic(forumId, _identityProvider.Current.UserId, title, cancellationToken);
        await scope.GetStorage<IDomainEventStorage>().AddEvent(result, cancellationToken);
        await scope.Commit(cancellationToken);

        return result;
    }
}