namespace Forums.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public ErrorCode ErrorCode { get; }

    public DomainException(ErrorCode errorCode, string message) 
        : base(message)
    {
        ErrorCode = errorCode;
    }
}