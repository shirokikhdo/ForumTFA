using Forums.Domain.UseCases.GetForums;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Storages;

public class GetForumsStorage : IGetForumsStorage
{
    private readonly ForumDbContext _dbContext;
    public GetForumsStorage(
        ForumDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken) =>
        await _dbContext.Forums
            .Select(f => new Domain.Models.Forum
            {
                Id = f.ForumId,
                Title = f.Title
            })
            .ToArrayAsync(cancellationToken);
}