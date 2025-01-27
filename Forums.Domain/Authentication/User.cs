namespace Forums.Domain.Authentication;

internal class User : IIdentity
{
    public Guid UserId { get; }

    public User(Guid userId)
    {
        UserId = userId;
    }
}