using Forums.Domain.Authentication;
using Forums.Domain.Authorization;

namespace Forums.Domain.UseCases.SignOut;

internal class AccountIntentionResolver : IIntentionResolver<AccountIntention>
{
    public bool IsAllowed(IIdentity subject, AccountIntention intention) => 
        intention switch 
        {
            AccountIntention.SignOut => subject.IsAuthenticated(),
            _ => false,
        };
}