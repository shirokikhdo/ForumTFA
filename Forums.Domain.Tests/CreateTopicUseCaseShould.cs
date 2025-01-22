using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using Moq.Language.Flow;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.Exceptions;
using Forums.Domain.UseCases.CreateTopic;
using Topic = Forums.Domain.Models.Topic;

namespace Forums.Domain.Tests;

public class CreateTopicUseCaseShould
{
    private readonly CreateTopicUseCase _sut;
    private readonly Mock<ICreateTopicStorage> _storage;
    private readonly ISetup<ICreateTopicStorage, Task<bool>> _forumExistsSetup;
    private readonly ISetup<ICreateTopicStorage, Task<Topic>> _createTopicSetup;
    private readonly ISetup<IIdentity, Guid> _getCurrentUserIdSetup;
    private readonly ISetup<IIntentionManager, bool> _intentionIsAllowedSetup;
    private readonly Mock<IIntentionManager> _intentionManager;

    public CreateTopicUseCaseShould()
    {
        _storage = new Mock<ICreateTopicStorage>();
        _forumExistsSetup = _storage.Setup(s => s.ForumExists(
            It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()));
        _createTopicSetup = _storage.Setup(s => s.CreateTopic(
            It.IsAny<Guid>(), 
            It.IsAny<Guid>(), 
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()));

        var identity = new Mock<IIdentity>();
        var identityProvider = new Mock<IIdentityProvider>();
        identityProvider.Setup(p => p.Current).Returns(identity.Object);
        _getCurrentUserIdSetup = identity.Setup(s => s.UserId);

        _intentionManager = new Mock<IIntentionManager>();
        _intentionIsAllowedSetup = _intentionManager.Setup(m => 
            m.IsAllowed(It.IsAny<TopicIntention>()));

        var validator = new Mock<IValidator<CreateTopicCommand>>();
        validator.Setup(v => 
                v.ValidateAsync(It.IsAny<CreateTopicCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        
        _sut = new CreateTopicUseCase(validator.Object, _intentionManager.Object, identityProvider.Object, _storage.Object);
    }

    [Fact]
    public async Task ThrowIntentionManagerException_WhenTopicCreationIsNotAllowed()
    {
        var forumId = Guid.Parse("3BB52FCF-FA8F-4DA3-9954-25A67F75B71A");

        _intentionIsAllowedSetup.Returns(false);

        var command = new CreateTopicCommand(forumId, "Whatever");

        await _sut.Invoking(s => s.Execute(command, CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();
        
        _intentionManager.Verify(m => 
            m.IsAllowed(TopicIntention.Create));
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoMatchingForum()
    {
        var forumId = Guid.Parse("5E1DCF96-E8F3-41C9-BD59-6479140933B3");
        
        _intentionIsAllowedSetup.Returns(true);
        _forumExistsSetup.ReturnsAsync(false);

        var command = new CreateTopicCommand(forumId, "Some title");

        await _sut.Invoking(s => s.Execute(command, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
        
        _storage.Verify(s => 
            s.ForumExists(forumId, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ReturnNewlyCreatedTopic_WhenMatchingForumExists()
    {
        var forumId = Guid.Parse("E20A64A3-47E3-4076-96D0-7EF226EAF5F2");
        var userId = Guid.Parse("91B714CC-BDFF-47A1-A6DC-E71DDE8C25F7");

        _intentionIsAllowedSetup.Returns(true);
        _forumExistsSetup.ReturnsAsync(true);
        _getCurrentUserIdSetup.Returns(userId);
        
        var expected = new Topic();
        _createTopicSetup.ReturnsAsync(expected);

        var command = new CreateTopicCommand(forumId, "Hello world");

        var actual = await _sut.Execute(command, CancellationToken.None);
        actual.Should().Be(expected);

        _storage.Verify(s => 
            s.CreateTopic(forumId, userId, "Hello world", It.IsAny<CancellationToken>()), Times.Once);
    }
}