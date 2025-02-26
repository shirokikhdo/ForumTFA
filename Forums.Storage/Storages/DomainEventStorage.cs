using Forums.Domain.UseCases;
using Forums.Storage.Entities;
using Forums.Storage.Models;
using System.Text.Json;

namespace Forums.Storage.Storages;

internal class DomainEventStorage 
    : IDomainEventStorage
{
    private readonly ForumDbContext _dbContext;
    private readonly IGuidFactory _guidFactory;
    private readonly IMomentProvider _momentProvider;

    public DomainEventStorage(
        ForumDbContext dbContext,
        IGuidFactory guidFactory,
        IMomentProvider momentProvider)
    {
        _dbContext = dbContext;
        _guidFactory = guidFactory;
        _momentProvider = momentProvider;
    }

    public async Task AddEvent<TDomainEntity>(TDomainEntity entity, CancellationToken cancellationToken)
    {
        await _dbContext.DomainEvents.AddAsync(
            new DomainEvent 
            {
                DomainEventId = _guidFactory.Create(),
                EmittedAt = _momentProvider.Now,
                ContentBlob = JsonSerializer.SerializeToUtf8Bytes(entity)

            }, 
            cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}