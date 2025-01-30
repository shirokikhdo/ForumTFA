namespace Forums.Domain.Authentication;

public interface IIdentityProvider
{
    IIdentity Current { get; set; }

    //IIdentity Current { get; }
}