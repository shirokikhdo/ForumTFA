using Forums.Domain.Models;

namespace Forums.Domain.UseCases.CreateForum;

public interface ICreateForumUseCase
{
    Task<Forum> Execute(CreateForumCommand command, CancellationToken cancellationToken);
}