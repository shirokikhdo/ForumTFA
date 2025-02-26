using Forums.Domain.Models;
using MediatR;

namespace Forums.Domain.UseCases.GetForums;

internal class GetForumsUseCase 
    : IRequestHandler<GetForumsQuery, IEnumerable<Forum>>
{
    private readonly IGetForumsStorage _storage;

    public GetForumsUseCase(
        IGetForumsStorage storage)
    {
        _storage = storage;
    }

    public Task<IEnumerable<Forum>> Handle(GetForumsQuery query, CancellationToken cancellationToken) => 
        _storage.GetForums(cancellationToken);
}