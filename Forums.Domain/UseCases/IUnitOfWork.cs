namespace Forums.Domain.UseCases;

public interface IUnitOfWork
{
    Task<IUnitOfWorkScope> StartScope(CancellationToken cancellationToken);
}