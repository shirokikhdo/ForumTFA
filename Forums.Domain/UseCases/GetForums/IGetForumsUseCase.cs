using Forums.Domain.Models;

namespace Forums.Domain.UseCases.GetForums;

public interface IGetForumsUseCase
{
    Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken);
}