using System.Security.Cryptography;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;
using Forums.Domain.Authentication;

namespace Forums.Domain.Tests.Authentication;

public class AuthenticationServiceShould
{
    private readonly AuthenticationService _sut;
    private readonly ISetup<ISymmetricDecryptor, Task<string>> _setupDecrypt;
    private readonly ISetup<IAuthenticationStorage, Task<Session?>> _findSessionSetup;

    public AuthenticationServiceShould()
    {
        var decryptor = new Mock<ISymmetricDecryptor>();
        _setupDecrypt = decryptor.Setup(d => d.Decrypt(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<CancellationToken>()));
        
        var storage = new Mock<IAuthenticationStorage>();
        _findSessionSetup = storage.Setup(s => s.FindSession(
            It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()));

        var options = new Mock<IOptions<AuthenticationConfiguration>>();
        options
            .Setup(o => o.Value)
            .Returns(new AuthenticationConfiguration
            {
                Base64Key = "XtDotH86WLjaEoFev6uZFN/3C0EQIApoD+5iqqmPtpg="
            });

        _sut = new AuthenticationService(
            decryptor.Object,
            storage.Object,
            NullLogger<AuthenticationService>.Instance,
            options.Object);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenCannotBeDecrypted()
    {
        _setupDecrypt.Throws<CryptographicException>();
        var actual = await _sut.Authenticate("hahaha-bad-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenTokenIsInvalid()
    {
        _setupDecrypt.ReturnsAsync("not-a-guid");
        var actual = await _sut.Authenticate("bad-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenSessionNotFound()
    {
        _setupDecrypt.ReturnsAsync("EE88598F-5896-4885-BE61-9B31171EAB9E");
        _findSessionSetup.ReturnsAsync(() => null);
        var actual = await _sut.Authenticate("good-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnGuestIdentity_WhenSessionIsExpired()
    {
        _setupDecrypt.ReturnsAsync("EE88598F-5896-4885-BE61-9B31171EAB9E");
        _findSessionSetup.ReturnsAsync(new Session
        {
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(-1)
        });
        var actual = await _sut.Authenticate("good-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(User.Guest);
    }

    [Fact]
    public async Task ReturnIdentity_WhenSessionIsValid()
    {
        var sessionId = Guid.Parse("B08B27F8-76DA-44B3-B264-E59A2858669E");
        var userId = Guid.Parse("7C4F18A3-52D8-473A-990E-BABD142876D9");
        _setupDecrypt.ReturnsAsync("B08B27F8-76DA-44B3-B264-E59A2858669E");
        _findSessionSetup.ReturnsAsync(new Session
        {
            UserId = userId,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(1)
        });
        var actual = await _sut.Authenticate("good-token", CancellationToken.None);
        actual.Should().BeEquivalentTo(new User(userId, sessionId));
    }
}