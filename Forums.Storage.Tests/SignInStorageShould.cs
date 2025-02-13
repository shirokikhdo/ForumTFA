using FluentAssertions;
using Forums.Storage.Models;
using Forums.Storage.Storages;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage.Tests;

public class SignInStorageShould : IClassFixture<SignInStorageFixture>
{
    private readonly SignInStorageFixture _fixture;
    private readonly SignInStorage _sut;

    public SignInStorageShould(SignInStorageFixture fixture)
    {
        _fixture = fixture;
        _sut = new SignInStorage(
            new GuidFactory(),
            fixture.GetDbContext(),
            fixture.GetMapper());
    }

    [Fact]
    public async Task ReturnUser_WhenDatabaseContainsUserWithSameLogin()
    {
        var actual = await _sut.FindUser("testUser", CancellationToken.None);

        actual.Should().NotBeNull();
        actual!.UserId.Should().Be(Guid.Parse("8B41C23E-123E-4F4A-93F0-BEBF9916C8B3"));
    }

    [Fact]
    public async Task ReturnNull_WhenDatabaseDoesntContainUserWithSameLogin()
    {
        var actual = await _sut.FindUser("whatever", CancellationToken.None);

        actual.Should().BeNull();
    }

    [Fact]
    public async Task ReturnNewlyCreatedSessionId()
    {
        var sessionId = await _sut.CreateSession(
            Guid.Parse("8B41C23E-123E-4F4A-93F0-BEBF9916C8B3"),
            new DateTimeOffset(2023, 10, 12, 19, 25, 00, TimeSpan.Zero),
            CancellationToken.None);

        await using var dbContext = _fixture.GetDbContext();

        (await dbContext.Sessions
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.SessionId == sessionId)).Should().NotBeNull();
    }
}