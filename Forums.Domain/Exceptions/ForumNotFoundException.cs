namespace Forums.Domain.Exceptions;

public class ForumNotFoundException : DomainException
{
    public ForumNotFoundException(Guid forumId) 
        : base(DomainErrorCode.Gone, $"Forum with id {forumId} was not found")
    {
        
    }
}