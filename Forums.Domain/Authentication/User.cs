namespace Forums.Domain.Authentication;

public class User : IIdentity
{
    public Guid UserId { get; }

    public User(Guid userId)
    {
        UserId = userId;
    }
}