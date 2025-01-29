using AutoMapper;
using AutoMapper.QueryableExtensions;
using Forums.Domain.UseCases.GetForums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Forums.Storage.Storages;

internal class GetForumsStorage : IGetForumsStorage
{
    private readonly IMemoryCache _memoryCache;
    private readonly ForumDbContext _dbContext;
    private readonly IMapper _mapper;

    public GetForumsStorage(
        IMemoryCache memoryCache,
        ForumDbContext dbContext,
        IMapper mapper)
    {
        _memoryCache = memoryCache;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<IEnumerable<Domain.Models.Forum>> GetForums(CancellationToken cancellationToken) =>
        await _memoryCache.GetOrCreateAsync<Domain.Models.Forum[]>(
            nameof(GetForums),
            entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);
                return _dbContext.Forums
                    .ProjectTo<Domain.Models.Forum>(_mapper.ConfigurationProvider)
                    .ToArrayAsync(cancellationToken);
            });
}