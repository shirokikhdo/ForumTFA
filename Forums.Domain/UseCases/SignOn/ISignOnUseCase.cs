using Forums.Domain.Authentication;

namespace Forums.Domain.UseCases.SignOn;

public interface ISignOnUseCase
{
    Task<IIdentity> Execute(SignOnCommand command, CancellationToken cancellationToken);
}