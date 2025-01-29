using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.E2E;

public class MapperConfigurationShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory _factory;

    public MapperConfigurationShould(
        ForumApiApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public void BeValid()
    {
        var configurationProvider = _factory.Services
            .GetRequiredService<IMapper>()
            .ConfigurationProvider;
        
        configurationProvider.Invoking(p => p.AssertConfigurationIsValid())
            .Should().NotThrow();
    }
}