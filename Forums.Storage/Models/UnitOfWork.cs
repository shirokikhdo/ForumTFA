using Forums.Domain.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.Storage.Models;

internal class UnitOfWork : IUnitOfWork
{
    private readonly IServiceProvider _serviceProvider;

    public UnitOfWork(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IUnitOfWorkScope> StartScope(CancellationToken cancellationToken)
    {
        var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ForumDbContext>();
        var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        return new UnitOfWorkScope(scope, transaction);
    }
}