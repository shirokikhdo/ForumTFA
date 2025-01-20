namespace Forums.Domain.Models;

public interface IMomentProvider
{
    DateTimeOffset Now { get; }
}