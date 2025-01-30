using Forums.Domain.Authentication;

namespace Forums.Domain.UseCases.SignIn;

public interface ISignInUseCase
{
    Task<(IIdentity identity, string token)> Execute(SignInCommand command, CancellationToken cancellationToken);
}