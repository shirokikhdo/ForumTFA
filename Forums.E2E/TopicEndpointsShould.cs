using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Forums.API.Models;

namespace Forums.E2E;

public class TopicEndpointsShould : IClassFixture<ForumApiApplicationFactory>, IAsyncLifetime
{
    private readonly ForumApiApplicationFactory _factory;

    public TopicEndpointsShould(ForumApiApplicationFactory factory)
    {
        _factory = factory;
    }

    public Task InitializeAsync() => 
        Task.CompletedTask;

    public Task DisposeAsync() => 
        Task.CompletedTask;

    //[Fact]
    public async Task ReturnForbidden_WhenNotAuthenticated()
    {
        using var httpClient = _factory.CreateClient();
        using var forumCreatedResponse = await httpClient.PostAsync("forums",
            JsonContent.Create(new { title = "Test forum" }));
        forumCreatedResponse.EnsureSuccessStatusCode();
        var createdForum = await forumCreatedResponse.Content.ReadFromJsonAsync<Forum>();
        createdForum.Should().NotBeNull();
        
        var responseMessage = await httpClient.PostAsync($"forums/{createdForum!.Id}/topics",
            JsonContent.Create(new { title = "Hello world" }));
        responseMessage.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
    }
}