using FluentAssertions;
using Moq;
using Moq.Language.Flow;
using Forums.Domain.Exceptions;
using Forums.Domain.Models;
using Forums.Domain.UseCases.GetForums;
using Forums.Domain.UseCases.GetTopics;

namespace Forums.Domain.Tests.GetTopics;

public class GetTopicsUseCaseShould
{
    private readonly GetTopicsUseCase _sut;
    private readonly Mock<IGetTopicsStorage> _storage;
    private readonly ISetup<IGetTopicsStorage, Task<(IEnumerable<Topic> resources, int totalCount)>> _getTopicsSetup;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> _getForumsSetup;

    public GetTopicsUseCaseShould()
    {
        var getForumsStorage = new Mock<IGetForumsStorage>();
        _getForumsSetup = getForumsStorage.Setup(s => s.GetForums(It.IsAny<CancellationToken>()));
        
        _storage = new Mock<IGetTopicsStorage>();
        _getTopicsSetup = _storage.Setup(s =>
            s.GetTopics(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()));

        _sut = new GetTopicsUseCase(getForumsStorage.Object, _storage.Object);
    }

    [Fact]
    public async Task ThrowForumNotFoundException_WhenNoForum()
    {
        var forumId = Guid.Parse("64C3B227-8D4A-4A0E-A161-04F19C2ABBC4");
        _getForumsSetup.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("01B1C554-184B-4B32-913E-F7031AAD3BAC") } });
        var query = new GetTopicsQuery(forumId, 0, 1);
        
        await _sut.Invoking(s => s.Handle(query, CancellationToken.None))
            .Should().ThrowAsync<ForumNotFoundException>();
    }
    [Fact]
    public async Task ReturnTopics_ExtractedFromStorage_WhenForumExists()
    {
        var forumId = Guid.Parse("845D0972-0E11-4BD2-A102-299E99590267");
        _getForumsSetup.ReturnsAsync(new Forum[] { new() { Id = Guid.Parse("845D0972-0E11-4BD2-A102-299E99590267") } });
        var expectedResources = new Topic[] { new() };
        var expectedTotalCount = 6;
        _getTopicsSetup.ReturnsAsync((expectedResources, expectedTotalCount));

        var query = new GetTopicsQuery(forumId, 5, 10);
        var (actualResources, actualTotalCount) = await _sut.Handle(
            query, CancellationToken.None);
        
        actualResources.Should().BeEquivalentTo(expectedResources);
        actualTotalCount.Should().Be(expectedTotalCount);
        
        _storage.Verify(s => s.GetTopics(forumId, 5, 10, It.IsAny<CancellationToken>()), Times.Once);
    }
}