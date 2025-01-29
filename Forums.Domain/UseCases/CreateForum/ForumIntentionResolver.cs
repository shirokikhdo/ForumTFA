using Forums.Domain.Authentication;
using Forums.Domain.Authorization;

namespace Forums.Domain.UseCases.CreateForum;

internal class ForumIntentionResolver : IIntentionResolver<ForumIntention>
{
    public bool IsAllowed(IIdentity subject, ForumIntention intention) => 
        intention switch 
        {
            ForumIntention.Create => subject.IsAuthenticated(),
            _ => false
        };
}