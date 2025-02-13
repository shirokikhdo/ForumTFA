using Forums.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;

namespace Forums.E2E;

public class ForumApiApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer; 

    public ForumApiApplicationFactory()
    {
        _dbContainer = new PostgreSqlBuilder().Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                ["ConnectionStrings:PostgresConnectionString"] = _dbContainer.GetConnectionString(),
                ["Authentication:Base64Key"] = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
            })
            .Build();
        builder.UseConfiguration(configuration);
        builder.ConfigureLogging(cfg => cfg.ClearProviders());
        base.ConfigureWebHost(builder);
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        var forumDbContext = new ForumDbContext(new DbContextOptionsBuilder<ForumDbContext>()
            .UseNpgsql(_dbContainer.GetConnectionString()).Options);
        await forumDbContext.Database.MigrateAsync();
    }
    
    public new async Task DisposeAsync() => 
        await _dbContainer.DisposeAsync();
}