namespace Forums.Domain.Models;

public class MomentProvider : IMomentProvider
{
    public DateTimeOffset Now 
        => DateTimeOffset.Now;
}