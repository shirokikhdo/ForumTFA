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
    private readonly ISetup<IIntentionManager, bool> _signOutIsAllowedSetup;

    public SignOutUseCaseShould()
    {
        _storage = new Mock<ISignOutStorage>();
        _removeSessionSetup = _storage.Setup(s => s.RemoveSession(
            It.IsAny<Guid>(), 
            It.IsAny<CancellationToken>()));

        var identityProvider = new Mock<IIdentityProvider>();
        _currentIdentitySetup = identityProvider.Setup(p => p.Current);

        var intentionManager = new Mock<IIntentionManager>();
        _signOutIsAllowedSetup = intentionManager.Setup(m => m.IsAllowed(It.IsAny<AccountIntention>()));
        
        _sut = new SignOutUseCase(
            intentionManager.Object,
            identityProvider.Object,
            _storage.Object);
    }

    [Fact]
    public async Task ThrowIntentionManagerException_WhenUserIsNotAuthenticated()
    {
        _signOutIsAllowedSetup.Returns(false);
        await _sut.Invoking(s => s.Execute(new SignOutCommand(), CancellationToken.None))
            .Should().ThrowAsync<IntentionManagerException>();
    }

    [Fact]
    public async Task RemoveCurrentIdentitySession()
    {
        _signOutIsAllowedSetup.Returns(true);
        var sessionId = Guid.Parse("DC933EC9-C7B4-4CAA-A3DE-A394FF55BBEF");
        _currentIdentitySetup.Returns(new User(Guid.Empty, sessionId));
        _removeSessionSetup.Returns(Task.CompletedTask);

        await _sut.Execute(new SignOutCommand(), CancellationToken.None);

        _storage.Verify(s => s.RemoveSession(
            sessionId, 
            It.IsAny<CancellationToken>()), 
            Times.Once);
        _storage.VerifyNoOtherCalls();
    }
}