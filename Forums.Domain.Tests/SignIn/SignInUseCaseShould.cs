using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Forums.Domain.Authentication;
using Forums.Domain.UseCases.SignIn;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Language.Flow;

namespace Forums.Domain.Tests.SignIn;

public class SignInUseCaseShould
{
    private readonly SignInUseCase _sut;
    private readonly ISetup<ISignInStorage, Task<RecognisedUser?>> _findUserSetup;
    private readonly ISetup<IPasswordManager, bool> _comparePasswordsSetup;
    private readonly ISetup<ISymmetricEncryptor, Task<string>> _encryptSetup;
    private readonly ISetup<ISignInStorage, Task<Guid>> _createSessionSetup;
    private readonly Mock<ISignInStorage> _storage;
    private readonly Mock<ISymmetricEncryptor> _encryptor;

    public SignInUseCaseShould()
    {
        var validator = new Mock<IValidator<SignInCommand>>();
        validator
            .Setup(v => v.ValidateAsync(It.IsAny<SignInCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        _storage = new Mock<ISignInStorage>();
        _findUserSetup = _storage.Setup(s => s.FindUser(
            It.IsAny<string>(), 
            It.IsAny<CancellationToken>()));
        _createSessionSetup = _storage.Setup(s => s.CreateSession(
            It.IsAny<Guid>(), 
            It.IsAny<DateTimeOffset>(), 
            It.IsAny<CancellationToken>()));

        var passwordManager = new Mock<IPasswordManager>();
        _comparePasswordsSetup = passwordManager.Setup(m => m.ComparePasswords(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<byte[]>()));

        _encryptor = new Mock<ISymmetricEncryptor>();
        _encryptSetup = _encryptor.Setup(e => e.Encrypt(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<CancellationToken>()));

        var configuration = new Mock<IOptions<AuthenticationConfiguration>>();
        configuration.Setup(c => c.Value)
            .Returns(new AuthenticationConfiguration
            {
                Base64Key = "XtDotH86WLjaEoFev6uZFN/3C0EQIApoD+5iqqmPtpg="
            });

        _sut = new SignInUseCase(
            validator.Object,
            _storage.Object,
            passwordManager.Object,
            _encryptor.Object,
            configuration.Object);
    }

    [Fact]
    public async Task ThrowValidationException_WhenUserNotFound()
    {
        _findUserSetup.ReturnsAsync(() => null);
        (await _sut.Invoking(s => s.Execute(new SignInCommand("Test", "qwerty"), CancellationToken.None))
                .Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == "Login");
    }

    [Fact]
    public async Task ThrowValidationException_WhenPasswordDoesntMatch()
    {
        _findUserSetup.ReturnsAsync(new RecognisedUser());
        _comparePasswordsSetup.Returns(false);
        (await _sut.Invoking(s => s.Execute(new SignInCommand("Test", "qwerty"), CancellationToken.None))
                .Should().ThrowAsync<ValidationException>())
            .Which.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [Fact]
    public async Task CreateSession_WhenPasswordMatches()
    {
        var userId = Guid.Parse("EA065C67-492D-446B-9B50-1D8EABF59BD6");
        var sessionId = Guid.Parse("1D5FD923-583D-4F1B-A305-7E2E1A6CFD54");
        _findUserSetup.ReturnsAsync(new RecognisedUser { UserId = userId });
        _comparePasswordsSetup.Returns(true);
        _createSessionSetup.ReturnsAsync(sessionId);

        await _sut.Execute(new SignInCommand("Test", "qwerty"), CancellationToken.None);

        _storage.Verify(s => s.CreateSession(
            userId, 
            It.IsAny<DateTimeOffset>(), 
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ReturnTokenAndIdentity()
    {
        var userId = Guid.Parse("154B1F4C-486F-49AA-9109-720DE5EB524D");
        var sessionId = Guid.Parse("9722BC76-FD4C-4E3F-927E-4DD1CABBA55E");
        _findUserSetup.ReturnsAsync(new RecognisedUser
        {
            UserId = userId,
            PasswordHash = new byte[] { 1 },
            Salt = new byte[] { 2 }
        });
        _comparePasswordsSetup.Returns(true);
        _createSessionSetup.ReturnsAsync(sessionId);
        _encryptSetup.ReturnsAsync("token");

        var (identity, token) = await _sut.Execute(new SignInCommand("Test", "qwerty"), CancellationToken.None);

        token.Should().NotBeEmpty();
        identity.UserId.Should().Be(userId);
        identity.SessionId.Should().Be(sessionId);
        token.Should().Be("token");
    }

    [Fact]
    public async Task EncryptSessionIdIntoToken()
    {
        var userId = Guid.Parse("EA065C67-492D-446B-9B50-1D8EABF59BD6");
        var sessionId = Guid.Parse("1d5fd923-583d-4f1b-a305-7e2e1a6cfd54");
        _findUserSetup.ReturnsAsync(new RecognisedUser { UserId = userId });
        _comparePasswordsSetup.Returns(true);
        _createSessionSetup.ReturnsAsync(sessionId);

        await _sut.Execute(new SignInCommand("Test", "qwerty"), CancellationToken.None);

        _encryptor.Verify(s => s.Encrypt(
            "1d5fd923-583d-4f1b-a305-7e2e1a6cfd54", 
            It.IsAny<byte[]>(), 
            It.IsAny<CancellationToken>()));
    }
}