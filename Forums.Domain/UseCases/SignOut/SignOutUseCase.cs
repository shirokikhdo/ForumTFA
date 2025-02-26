using Forums.Domain.Authentication;
using MediatR;

namespace Forums.Domain.UseCases.SignOut;

internal class SignOutUseCase 
    : IRequestHandler<SignOutCommand>
{
    private readonly IIdentityProvider _identityProvider;
    private readonly ISignOutStorage _storage;

    public SignOutUseCase(
        IIdentityProvider identityProvider,
        ISignOutStorage storage)
    {
        _identityProvider = identityProvider;
        _storage = storage;
    }

    public Task Handle(SignOutCommand command, CancellationToken cancellationToken) => 
        _storage.RemoveSession(_identityProvider.Current.SessionId, cancellationToken);
}