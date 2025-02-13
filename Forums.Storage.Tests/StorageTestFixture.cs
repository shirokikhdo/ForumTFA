using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Testcontainers.PostgreSql;

namespace Forums.Storage.Tests;

public class StorageTestFixture : IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;

    public StorageTestFixture()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
    }

    public ForumDbContext GetDbContext() =>
        new ForumDbContext(
            new DbContextOptionsBuilder<ForumDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString()).Options);
    
    public IMapper GetMapper() => 
        new Mapper(
            new MapperConfiguration(cfg => 
                cfg.AddMaps(Assembly.GetAssembly(typeof(ForumDbContext)))));

    public IMemoryCache GetMemoryCache() => 
        new MemoryCache(
            new MemoryCacheOptions());

    public virtual async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var forumDbContext = new ForumDbContext(
            new DbContextOptionsBuilder<ForumDbContext>()
                .UseNpgsql(_dbContainer.GetConnectionString()).Options);
        await forumDbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync() => 
        await _dbContainer.DisposeAsync();
}