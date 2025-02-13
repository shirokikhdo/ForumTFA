using FluentAssertions;
using Forums.Storage.Models;
using Forums.Storage.Storages;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Tests;

public class CreateForumStorageShould : IClassFixture<StorageTestFixture>
{
    private readonly StorageTestFixture _fixture;
    private readonly CreateForumStorage _sut;

    public CreateForumStorageShould(StorageTestFixture fixture)
    {
        _fixture = fixture;
        _sut = new CreateForumStorage(
            fixture.GetMemoryCache(), 
            new GuidFactory(), 
            fixture.GetDbContext(), 
            fixture.GetMapper());
    }

    [Fact]
    public async Task InsertNewForumInDatabase()
    {
        var forum = await _sut.Create("Test title", CancellationToken.None);
        forum.Id.Should().NotBeEmpty();

        await using var dbContext = _fixture.GetDbContext();
        var forumTitles = await dbContext.Forums
            .Where(f => f.ForumId == forum.Id)
            .Select(f => f.Title).ToArrayAsync();

        forumTitles.Should().HaveCount(1).And.Contain("Test title");
    }
}