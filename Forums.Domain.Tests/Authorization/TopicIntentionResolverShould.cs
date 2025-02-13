using FluentAssertions;
using Forums.Domain.Authentication;
using Forums.Domain.UseCases.CreateTopic;
using Moq;

namespace Forums.Domain.Tests.Authorization;

public class TopicIntentionResolverShould
{
    private readonly TopicIntentionResolver _sut;

    public TopicIntentionResolverShould()
    {
        _sut = new TopicIntentionResolver();
    }

    [Fact]
    public void ReturnFalse_WhenIntentionNotInEnum()
    {
        var intention = (TopicIntention)(-1);
        _sut.IsAllowed(new Mock<IIdentity>().Object, intention).Should().BeFalse();
    }

    [Fact]
    public void ReturnFalse_WhenCheckingTopicCreateIntention_AndUserIsGuest()
    {
        _sut.IsAllowed(User.Guest, TopicIntention.Create).Should().BeFalse();
    }

    [Fact]
    public void ReturnTrue_WhenCheckingTopicCreateIntention_AndUserIsAuthenticated()
    {
        var user = new User(Guid.Parse("6F5C56BD-25EB-4BDC-9604-F19DAE2963A4"), Guid.Empty);
        _sut.IsAllowed(user, TopicIntention.Create).Should().BeTrue();
    }
}