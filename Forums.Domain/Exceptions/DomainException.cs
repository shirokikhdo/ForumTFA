namespace Forums.Domain.Exceptions;

public abstract class DomainException : Exception
{
    public DomainErrorCode DomainErrorCode { get; }

    public DomainException(DomainErrorCode domainErrorCode, string message) 
        : base(message)
    {
        DomainErrorCode = domainErrorCode;
    }
}