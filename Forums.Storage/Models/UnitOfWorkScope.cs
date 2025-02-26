using Forums.Domain.UseCases;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.Storage.Models;

internal class UnitOfWorkScope : IUnitOfWorkScope
{
    private readonly IServiceScope _scope;
    private readonly IDbContextTransaction _transaction;

    public UnitOfWorkScope(
        IServiceScope scope,
        IDbContextTransaction transaction)
    {
        _scope = scope;
        _transaction = transaction;
    }

    public TStorage GetStorage<TStorage>() where TStorage : IStorage =>
        _scope.ServiceProvider.GetRequiredService<TStorage>();

    public Task Commit(CancellationToken cancellationToken) =>
        _transaction.CommitAsync(cancellationToken);

    public async ValueTask DisposeAsync()
    {
        await _transaction.DisposeAsync();

        if (_scope is IAsyncDisposable scopeAsyncDisposable)
            await scopeAsyncDisposable.DisposeAsync();

        else
            _scope.Dispose();
    }
}