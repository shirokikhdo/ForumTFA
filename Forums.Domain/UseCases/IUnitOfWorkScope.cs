namespace Forums.Domain.UseCases;

public interface IUnitOfWorkScope : IAsyncDisposable
{
    TStorage GetStorage<TStorage>() 
        where TStorage : IStorage;

    Task Commit(CancellationToken cancellationToken);
}