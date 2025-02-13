using FluentAssertions;
using Forums.Domain.Authentication;
using Forums.Domain.UseCases.SignOut;
using Moq;

namespace Forums.Domain.Tests.Authorization;

public class AccountIntentionResolverShould
{
    private readonly AccountIntentionResolver _sut;

    public AccountIntentionResolverShould()
    {
        _sut = new AccountIntentionResolver();
    }

    [Fact]
    public void ReturnFalse_WhenIntentionNotInEnum()
    {
        var intention = (AccountIntention)(-1);
        _sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingForumCreateIntention_AndUserIsGuest()
    {
        _sut.IsAllowed(User.Guest, AccountIntention.SignOut).Should().BeFalse();
    }

    [Fact]
    public void ReturnTrue_WhenCheckingForumCreateIntention_AndUserIsAuthenticated()
    {
        var user = new User(Guid.Parse("6F5C56BD-25EB-4BDC-9604-F19DAE2963A4"), Guid.Empty);
        _sut.IsAllowed(user, AccountIntention.SignOut).Should().BeTrue();
    }
}