using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.E2E;

public class MapperConfigurationShould : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public MapperConfigurationShould(
        WebApplicationFactory<Program> factory)
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