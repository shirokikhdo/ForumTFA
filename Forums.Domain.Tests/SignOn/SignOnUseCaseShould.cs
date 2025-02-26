using FluentAssertions;
using Forums.Domain.Authentication;
using Forums.Domain.UseCases.SignOn;
using Moq;
using Moq.Language.Flow;

namespace Forums.Domain.Tests.SignOn;

public class SignOnUseCaseShould
{
    private readonly SignOnUseCase _sut;
    private readonly ISetup<IPasswordManager, (byte[] Salt, byte[] Hash)> _generatePasswordPartsSetup;
    private readonly ISetup<ISignOnStorage, Task<Guid>> _createUserSetup;
    private readonly Mock<ISignOnStorage> _storage;

    public SignOnUseCaseShould()
    {
        var passwordManager = new Mock<IPasswordManager>();
        _generatePasswordPartsSetup = passwordManager.Setup(m => 
            m.GeneratePasswordParts(It.IsAny<string>()));
        
        _storage = new Mock<ISignOnStorage>();
        _createUserSetup = _storage.Setup(s => s.CreateUser(
            It.IsAny<string>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<byte[]>(), 
            It.IsAny<CancellationToken>()));

        _sut = new SignOnUseCase(passwordManager.Object, _storage.Object);
    }

    [Fact]
    public async Task CreateUser_WithGeneratedPasswordParts()
    {
        var salt = new byte[] { 1 };
        var hash = new byte[] { 2 };
        _generatePasswordPartsSetup.Returns((Salt: salt, Hash: hash));

        var command = new SignOnCommand("Test", "qwerty");
        await _sut.Handle(command, CancellationToken.None);

        _storage.Verify(s => s.CreateUser(
            "Test",
            salt, 
            hash, 
            It.IsAny<CancellationToken>()), 
            Times.Once);
        _storage.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task ReturnIdentityOfNewlyCreatedUser()
    {
        _generatePasswordPartsSetup.Returns((Salt: new byte[] { 1 }, Hash: new byte[] { 2 }));
        _createUserSetup.ReturnsAsync(Guid.Parse("7483221E-FE0E-44EE-85B6-94D5279A8988"));

        var command = new SignOnCommand("Test", "qwerty");
        var actual = await _sut.Handle(command, CancellationToken.None);

        actual.UserId.Should().Be(Guid.Parse("7483221E-FE0E-44EE-85B6-94D5279A8988"));
    }
}