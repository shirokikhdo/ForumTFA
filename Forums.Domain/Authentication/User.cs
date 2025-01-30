namespace Forums.Domain.Authentication;

public class User : IIdentity
{
    public Guid UserId { get; }

    public static User Guest => 
        new User(Guid.Empty);

    public User(Guid userId)
    {
        UserId = userId;
    }
}