using Forums.Domain.Monitoring;
using Forum = Forums.Domain.Models.Forum;

namespace Forums.Domain.UseCases.GetForums;

internal class GetForumsUseCase : IGetForumsUseCase
{
    private readonly IGetForumsStorage _storage;
    private readonly DomainMetrics _metrics;

    public GetForumsUseCase(
        IGetForumsStorage storage,
        DomainMetrics metrics)
    {
        _storage = storage;
        _metrics = metrics;
    }

    public async Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken)
    {
        try
        {
            var result = await _storage.GetForums(cancellationToken);
            _metrics.ForumsFetched(true);
            return result;
        }
        catch
        {
            _metrics.ForumsFetched(false);
            throw;
        }
    }
}