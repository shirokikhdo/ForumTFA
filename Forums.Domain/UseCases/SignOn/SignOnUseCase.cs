using Forums.Domain.Authentication;
using MediatR;

namespace Forums.Domain.UseCases.SignOn;

internal class SignOnUseCase 
    : IRequestHandler<SignOnCommand, IIdentity>
{
    private readonly IPasswordManager _passwordManager;
    private readonly ISignOnStorage _storage;

    public SignOnUseCase(
        IPasswordManager passwordManager,
        ISignOnStorage storage)
    {
        _passwordManager = passwordManager;
        _storage = storage;
    }

    public async Task<IIdentity> Handle(SignOnCommand command, CancellationToken cancellationToken)
    {
        var (salt, hash) = _passwordManager.GeneratePasswordParts(command.Password);

        var userId = await _storage.CreateUser(command.Login, salt, hash, cancellationToken);

        return new User(userId, Guid.Empty);
    }
}