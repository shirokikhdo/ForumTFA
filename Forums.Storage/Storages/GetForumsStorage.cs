using Forums.Domain.UseCases.GetForums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Forums.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly IMemoryCache _memoryCache;
    private readonly ForumDbContext _dbContext;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext dbContext)
    {
        _memoryCache = memoryCache;
        _dbContext = dbContext;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken) =>
        await _memoryCache.GetOrCreateAsync<Domain.Models.Forum[]>(
            nameof(GetForums),
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return _dbContext.Forums
                    .Select(f => new Domain.Models.Forum
                    {
                        Id = f.ForumId,
                        Title = f.Title
                    })
                    .ToArrayAsync(cancellationToken);
            });
}