using Forums.Domain.Authentication;
using Forums.Domain.Authorization;

namespace Forums.Domain.UseCases.CreateTopic;

public class TopicIntentionResolver : IIntentionResolver<TopicIntention>
{
    public bool IsAllowed(IIdentity subject, TopicIntention intention) => 
        intention switch 
        {
            TopicIntention.Create => subject.IsAuthenticated(),
            _ => false
        };
}