using Forums.Domain.UseCases.SignOn;
using Forums.Storage.Models;

namespace Forums.Storage.Storages;

internal class SignOnStorage : ISignOnStorage
{
    private readonly ForumDbContext _dbContext;
    private readonly IGuidFactory _guidFactory;
    
    public SignOnStorage(
        ForumDbContext dbContext,
        IGuidFactory guidFactory)
    {
        _dbContext = dbContext;
        _guidFactory = guidFactory;
    }

    public async Task<Guid> CreateUser(string login, byte[] salt, byte[] hash, CancellationToken cancellationToken)
    {
        var userId = _guidFactory.Create();

        await _dbContext.Users.AddAsync(new User
        {
            UserId = userId,
            Login = login,
            Salt = salt,
            PasswordHash = hash,
        }, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return userId;
    }
}