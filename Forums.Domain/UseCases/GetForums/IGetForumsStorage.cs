using Forums.Domain.Models;

namespace Forums.Domain.UseCases.GetForums;

public interface IGetForumsStorage
{
    Task<IEnumerable<Forum>> GetForums(CancellationToken cancellationToken);
}