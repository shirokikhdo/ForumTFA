using AutoMapper;
using AutoMapper.QueryableExtensions;
using Forums.Domain.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Storages;

internal class AuthenticationStorage : IAuthenticationStorage
{
    private readonly ForumDbContext _dbContext;
    private readonly IMapper _mapper;

    public AuthenticationStorage(
        ForumDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<Session?> FindSession(Guid sessionId, CancellationToken cancellationToken) =>
        _dbContext.Sessions
            .Where(s => s.SessionId == sessionId)
            .ProjectTo<Session>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
}