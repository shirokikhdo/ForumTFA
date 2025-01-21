using Forums.Domain.Models;

namespace Forums.Domain.UseCases.CreateTopic;

public interface ICreateTopicStorage
{
    Task<bool> ForumExists(Guid forumId, CancellationToken cancellationToken);

    Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}