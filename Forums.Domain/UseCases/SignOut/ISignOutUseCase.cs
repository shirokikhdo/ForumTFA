namespace Forums.Domain.UseCases.SignOut;

public interface ISignOutUseCase
{
    Task Execute(SignOutCommand command, CancellationToken cancellationToken);
}