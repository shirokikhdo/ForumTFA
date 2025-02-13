namespace Forums.Domain.Authentication;

public class User : IIdentity
{
    public Guid UserId { get; }

    public Guid SessionId { get; }

    public static User Guest => 
        new User(Guid.Empty, Guid.Empty);

    public User(Guid userId, Guid sessionId)
    {
        UserId = userId;
        SessionId = sessionId;
    }
}