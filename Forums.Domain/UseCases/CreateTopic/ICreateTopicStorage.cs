using Forums.Domain.Models;

namespace Forums.Domain.UseCases.CreateTopic;

public interface ICreateTopicStorage : IStorage
{
    Task<Topic> CreateTopic(Guid forumId, Guid userId, string title, CancellationToken cancellationToken);
}