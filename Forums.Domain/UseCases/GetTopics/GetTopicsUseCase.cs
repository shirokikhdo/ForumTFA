using Forums.Domain.Models;
using Forums.Domain.UseCases.GetForums;
using MediatR;

namespace Forums.Domain.UseCases.GetTopics;

internal class GetTopicsUseCase 
    : IRequestHandler<GetTopicsQuery, (IEnumerable<Topic> resources, int totalCount)>
{
    private readonly IGetForumsStorage _getForumsStorage;
    private readonly IGetTopicsStorage _storage;

    public GetTopicsUseCase(
        IGetForumsStorage getForumsStorage,
        IGetTopicsStorage storage)
    {
        _getForumsStorage = getForumsStorage;
        _storage = storage;
    }

    public async Task<(IEnumerable<Topic> resources, int totalCount)> Handle(
        GetTopicsQuery query, CancellationToken cancellationToken)
    {
        await _getForumsStorage.ThrowIfForumNotFound(query.ForumId, cancellationToken);
        return await _storage.GetTopics(query.ForumId, query.Skip, query.Take, cancellationToken);
    }
}