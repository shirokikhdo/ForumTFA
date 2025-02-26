using Forums.Domain.Authorization;
using Forums.Domain.Models;
using MediatR;

namespace Forums.Domain.UseCases.CreateForum;

internal class CreateForumUseCase 
    : IRequestHandler<CreateForumCommand, Forum>
{
    private readonly IIntentionManager _intentionManager;
    private readonly ICreateForumStorage _storage;

    public CreateForumUseCase(
        IIntentionManager intentionManager,
        ICreateForumStorage storage)
    {
        _intentionManager = intentionManager;
        _storage = storage;
    }

    public async Task<Forum> Handle(CreateForumCommand command, CancellationToken cancellationToken)
    {
        _intentionManager.ThrowIfForbidden(ForumIntention.Create);
        return await _storage.Create(command.Title, cancellationToken);
    }
}