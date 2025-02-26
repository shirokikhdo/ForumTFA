using FluentAssertions;
using Forums.Domain.Authentication;
using Forums.Domain.Authorization;
using Forums.Domain.UseCases.SignOut;
using Moq;
using Moq.Language.Flow;

namespace Forums.Domain.Tests.SignOut;

public class SignOutUseCaseShould
{
    private readonly SignOutUseCase _sut;
    private readonly Mock<ISignOutStorage> _storage;
    private readonly ISetup<ISignOutStorage, Task> _removeSessionSetup;
    private readonly ISetup<IIdentityProvider, IIdentity> _currentIdentitySetup;
    
    public SignOutUseCaseShould()
    {
        _storage = new Mock<ISignOutStorage>();
        _removeSessionSetup = _storage.Setup(s => s.RemoveSession(
            It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()));

        var identityProvider = new Mock<IIdentityProvider>();
        _currentIdentitySetup = identityProvider.Setup(p => p.Current);

        _sut = new SignOutUseCase(
            identityProvider.Object,
            _storage.Object);
    }

    [Fact]
    public async Task RemoveCurrentIdentitySession()
    {
        var sessionId = Guid.Parse("DC933EC9-C7B4-4CAA-A3DE-A394FF55BBEF");
        _currentIdentitySetup.Returns(new User(Guid.Empty, sessionId));
        _removeSessionSetup.Returns(Task.CompletedTask);

        var command = new SignOutCommand();
        await _sut.Handle(command, CancellationToken.None);

        _storage.Verify(s => s.RemoveSession(
            sessionId,  
            It.IsAny<CancellationToken>()), 
            Times.Once);
        _storage.VerifyNoOtherCalls();
    }
}