namespace Forums.Storage.Models;

internal class MomentProvider : IMomentProvider
{
    public DateTimeOffset Now 
        => DateTimeOffset.UtcNow;
}