using Forums.Domain.Exceptions;
using Forums.Domain.Models;
using Forums.Storage;
using Microsoft.EntityFrameworkCore;
using Topic = Forums.Domain.Models.Topic;

namespace Forums.Domain.UseCases.CreateTopic;

public class CreateTopicUseCase : ICreateTopicUseCase
{
    private readonly IGuidFactory _guidFactory;
    private readonly IMomentProvider _momentProvider;
    private readonly ForumDbContext _dbContext;

    public CreateTopicUseCase(
        IGuidFactory guidFactory,
        IMomentProvider momentProvider,
        ForumDbContext dbContext)
    {
        _guidFactory = guidFactory;
        _momentProvider = momentProvider;
        _dbContext = dbContext;
    }

    public async Task<Topic> Execute(
        Guid forumId, 
        string title, 
        Guid authorId, 
        CancellationToken cancellationToken)
    {
        var forumExists = await _dbContext.Forums
            .AnyAsync(f => f.ForumId == forumId, cancellationToken);
        
        if (!forumExists)
            throw new ForumNotFoundException(forumId);

        var topicId = _guidFactory.Create();
        
        await _dbContext.Topics.AddAsync(new Storage.Topic
        {
            TopicId = topicId,
            ForumId = forumId,
            UserId = authorId,
            CreatedAt = _momentProvider.Now,
            Title = title
        }, cancellationToken);
        
        await _dbContext.SaveChangesAsync(cancellationToken);

        var result = await _dbContext.Topics
            .Where(t => t.TopicId == topicId)
            .Select(t => new Topic
            {
                Id = t.TopicId,
                Title = t.Title,
                CreatedAt = t.CreatedAt,
                Author = t.Author.Login
            })
            .FirstAsync(cancellationToken);

        return result;
    }
}