namespace Forums.Domain.Authentication;

public static class IdentityExtensions
{
    public static bool IsAuthenticated(this IIdentity identity) => 
        identity.UserId != Guid.Empty;
}