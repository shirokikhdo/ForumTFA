using AutoMapper.QueryableExtensions;
using AutoMapper;
using Forums.Domain.UseCases.SignIn;
using Forums.Storage.Entities;
using Microsoft.EntityFrameworkCore;
using Forums.Storage.Models;

namespace Forums.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly IGuidFactory _guidFactory;
    private readonly ForumDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public SignInStorage(
        IGuidFactory guidFactory,
        ForumDbContext dbContext,
        IMapper mapper)
    {
        _guidFactory = guidFactory;
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken) => 
        _dbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Guid> CreateSession(
        Guid userId, 
        DateTimeOffset expirationMoment, 
        CancellationToken cancellationToken)
    {
        var sessionId = _guidFactory.Create();
        var session = new Session
        {
            SessionId = sessionId,
            UserId = userId,
            ExpiresAt = expirationMoment,
        };

        await _dbContext.Sessions.AddAsync(session, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return sessionId;
    }
}