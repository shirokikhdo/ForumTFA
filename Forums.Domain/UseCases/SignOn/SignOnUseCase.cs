using FluentValidation;
using Forums.Domain.Authentication;

namespace Forums.Domain.UseCases.SignOn;

internal class SignOnUseCase : ISignOnUseCase
{
    private readonly IValidator<SignOnCommand> _validator;
    private readonly IPasswordManager _passwordManager;
    private readonly ISignOnStorage _storage;

    public SignOnUseCase(
        IValidator<SignOnCommand> validator,
        IPasswordManager passwordManager,
        ISignOnStorage storage)
    {
        _validator = validator;
        _passwordManager = passwordManager;
        _storage = storage;
    }

    public async Task<IIdentity> Execute(SignOnCommand command, CancellationToken cancellationToken)
    {
        await _validator.ValidateAndThrowAsync(command, cancellationToken);

        var (salt, hash) = _passwordManager.GeneratePasswordParts(command.Password);

        var userId = await _storage.CreateUser(command.Login, salt, hash, cancellationToken);

        return new User(userId, Guid.Empty);
    }
}