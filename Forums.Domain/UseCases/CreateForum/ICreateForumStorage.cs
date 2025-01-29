using Forums.Domain.Models;

namespace Forums.Domain.UseCases.CreateForum;

public interface ICreateForumStorage
{
    public Task<Forum> Create(string title, CancellationToken cancellationToken);
}