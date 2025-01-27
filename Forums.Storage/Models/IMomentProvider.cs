namespace Forums.Storage.Models;

internal interface IMomentProvider
{
    DateTimeOffset Now { get; }
}