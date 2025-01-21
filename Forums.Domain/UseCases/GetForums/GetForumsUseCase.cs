using Forum = Forums.Domain.Models.Forum;

namespace Forums.Domain.UseCases.GetForums;

public class GetForumsUseCase : IGetForumsUseCase
{
    private readonly IGetForumsStorage _storage;

    public GetForumsUseCase(IGetForumsStorage storage)
    {
        _storage = storage;
    }

    public Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken) =>
        _storage.GetForums(cancellationToken);
}