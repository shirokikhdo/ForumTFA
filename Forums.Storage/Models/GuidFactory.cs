namespace Forums.Storage.Models;

internal class GuidFactory : IGuidFactory
{
    public Guid Create() =>
        Guid.NewGuid();
}