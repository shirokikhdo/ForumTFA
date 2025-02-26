using FluentAssertions;
using Forums.Domain.Authorization;
using Forums.Domain.Models;
using Forums.Domain.UseCases.CreateForum;
using Moq;
using Moq.Language.Flow;

namespace Forums.Domain.Tests.CreateForum;

public class CreateForumUseCaseShould
{
    private readonly CreateForumUseCase _sut;
    private readonly Mock<ICreateForumStorage> _storage;
    private readonly ISetup<ICreateForumStorage, Task<Forum>> _createForumSetup;

    public CreateForumUseCaseShould()
    {
        var intentionManager = new Mock<IIntentionManager>();
        intentionManager
            .Setup(m => m.IsAllowed(It.IsAny<ForumIntention>()))
            .Returns(true);

        _storage = new Mock<ICreateForumStorage>();
        _createForumSetup = _storage.Setup(s => s.Create(
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()));

        _sut = new CreateForumUseCase(intentionManager.Object, _storage.Object);
    }

    [Fact]
    public async Task ReturnCreatedForum()
    {
        var forum = new Forum
        {
            Id = Guid.Parse("F7958512-2306-4E49-BE20-FC0698536125"),
            Title = "Hello"
        };
        _createForumSetup.ReturnsAsync(forum);

        var command = new CreateForumCommand("Hello");
        var actual = await _sut.Handle(command, CancellationToken.None);
        actual.Should().Be(forum);

        _storage.Verify(s => s.Create("Hello", It.IsAny<CancellationToken>()), Times.Once);
        _storage.VerifyNoOtherCalls();
    }
}