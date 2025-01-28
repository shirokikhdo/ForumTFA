using Forums.Domain.UseCases.CreateTopic;
using Forums.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Storages;

internal class CreateTopicStorage : ICreateTopicStorage
{
    private readonly IGuidFactory _guidFactory;
    private readonly IMomentProvider _momentProvider;
    private readonly ForumDbContext _dbContext;

    public CreateTopicStorage(
        IGuidFactory guidFactory,
        IMomentProvider momentProvider,
        ForumDbContext dbContext)
    {
        _guidFactory = guidFactory;
        _momentProvider = momentProvider;
        _dbContext = dbContext;
    }

    public async Task<Domain.Models.Topic> CreateTopic(
        Guid forumId, 
        Guid userId, 
        string title,
        CancellationToken cancellationToken)
    {
        var topicId = _guidFactory.Create();
        var topic = new Topic
        {
            TopicId = topicId,
            ForumId = forumId,
            UserId = userId,
            Title = title,
            CreatedAt = _momentProvider.Now,
        };

        await _dbContext.Topics.AddAsync(topic, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = await _dbContext.Topics
            .Where(t => t.TopicId == topicId)
            .Select(t => new Domain.Models.Topic
            {
                Id = t.TopicId,
                ForumId = t.ForumId,
                UserId = t.UserId,
                Title = t.Title,
                CreatedAt = t.CreatedAt
            })
            .FirstAsync(cancellationToken);

        return result;
    }
}