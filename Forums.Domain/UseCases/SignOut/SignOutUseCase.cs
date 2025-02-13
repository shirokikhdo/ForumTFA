using Forums.Domain.Authentication;
using Forums.Domain.Authorization;

namespace Forums.Domain.UseCases.SignOut;

internal class SignOutUseCase : ISignOutUseCase
{
    private readonly IIntentionManager _intentionManager;
    private readonly IIdentityProvider _identityProvider;
    private readonly ISignOutStorage _storage;

    public SignOutUseCase(
        IIntentionManager intentionManager,
        IIdentityProvider identityProvider,
        ISignOutStorage storage)
    {
        _intentionManager = intentionManager;
        _identityProvider = identityProvider;
        _storage = storage;
    }

    public async Task Execute(SignOutCommand command, CancellationToken cancellationToken)
    {
        _intentionManager.ThrowIfForbidden(AccountIntention.SignOut);
        var sessionId = _identityProvider.Current.SessionId;
        await _storage.RemoveSession(sessionId, cancellationToken);
    }
}