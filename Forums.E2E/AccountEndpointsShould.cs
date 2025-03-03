﻿using System.Net.Http.Json;
using FluentAssertions;
using Forums.API.Models;
using Forums.Domain.Authentication;
using Forums.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Forums.E2E;

public class AccountEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory _factory;

    public AccountEndpointsShould(ForumApiApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task SignInAfterSignOn()
    {
        using var httpClient = _factory.CreateClient();

        using var signOnResponse = await httpClient.PostAsync(
            "account", JsonContent.Create(new { login = "Test", password = "qwerty" }));
        signOnResponse.IsSuccessStatusCode.Should().BeTrue();

        var createdUser = await signOnResponse.Content.ReadFromJsonAsync<User>();
        using var signInResponse = await httpClient.PostAsync(
            "account/signin", JsonContent.Create(new { login = "Test", password = "qwerty" }));
        signInResponse.IsSuccessStatusCode.Should().BeTrue();
        
        var signedInUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        
        signedInUser!.UserId.Should().Be(createdUser!.UserId);

        var createForumResponse = await httpClient.PostAsync(
            "forums", 
            JsonContent.Create(new { title = "Test title" }));

        createForumResponse.IsSuccessStatusCode.Should().BeTrue();

        var forum = (await createForumResponse.Content.ReadFromJsonAsync<Forum>())!;

        var createTopicResponse = await httpClient.PostAsync(
            $"forums/{forum.Id}/topics", 
            JsonContent.Create(new { title = "New topic" }));
        createTopicResponse.IsSuccessStatusCode.Should().BeTrue();

        await using var scope = _factory.Services.CreateAsyncScope();
        var domainEvents = await scope.ServiceProvider
            .GetRequiredService<ForumDbContext>()
            .DomainEvents
            .ToArrayAsync();
        domainEvents.Should().HaveCount(1);
    }
}