namespace Forums.Domain.Models;

public class GuidFactory : IGuidFactory
{
    public Guid Create() =>
        Guid.NewGuid();
}