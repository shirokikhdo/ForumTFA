using FluentAssertions;
using Forums.Domain.Authentication;
using Forums.Domain.UseCases.CreateForum;
using Moq;

namespace Forums.Domain.Tests.Authorization;

public class ForumIntentionResolverShould
{
    private readonly ForumIntentionResolver _sut;

    public ForumIntentionResolverShould()
    {
        _sut = new ForumIntentionResolver();
    }

    [Fact]
    public void ReturnFalse_WhenIntentionNotInEnum()
    {
        var intention = (ForumIntention)(-1);
        _sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingForumCreateIntention_AndUserIsGuest()
    {
        _sut.IsAllowed(User.Guest, ForumIntention.Create).Should().BeFalse();
    }

    [Fact]
    public void ReturnTrue_WhenCheckingForumCreateIntention_AndUserIsAuthenticated()
    {
        var user = new User(Guid.Parse("6F5C56BD-25EB-4BDC-9604-F19DAE2963A4"), Guid.Empty);
        _sut.IsAllowed(user, ForumIntention.Create).Should().BeTrue();
    }
}