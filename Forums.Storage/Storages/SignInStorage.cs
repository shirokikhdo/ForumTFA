using AutoMapper.QueryableExtensions;
using AutoMapper;
using Forums.Domain.UseCases.SignIn;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Storages;

internal class SignInStorage : ISignInStorage
{
    private readonly ForumDbContext _dbContext;
    private readonly IMapper _mapper;
    
    public SignInStorage(
        ForumDbContext dbContext,
        IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public Task<RecognisedUser?> FindUser(string login, CancellationToken cancellationToken) => 
        _dbContext.Users
            .Where(u => u.Login.Equals(login))
            .ProjectTo<RecognisedUser>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
}