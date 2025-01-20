using Forums.Storage;
using Microsoft.EntityFrameworkCore;
using Forum = Forums.Domain.Models.Forum;

namespace Forums.Domain.UseCases.GetForums;

public class GetForumsUseCase : IGetForumsUseCase
{
    private readonly ForumDbContext _dbContext;

    public GetForumsUseCase(ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Forum>> Execute(CancellationToken cancellationToken) =>
        await _dbContext.Forums
            .Select(f => new Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .ToArrayAsync(cancellationToken);
}