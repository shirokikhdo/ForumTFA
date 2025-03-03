﻿using System.Net.Http.Json;
using FluentAssertions;
using Forums.API.Models;

namespace Forums.E2E;

public class ForumEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory _factory;

    public ForumEndpointsShould(ForumApiApplicationFactory factory)
    {
        _factory = factory;
    }

    //[Fact]
    public async Task CreateNewForum()
    {
        const string forumTitle = "0069517D-CA29-453B-BB4C-AC22F51E690E";

        using var httpClient = _factory.CreateClient();
        
        using var getInitialForumsResponse = await httpClient.GetAsync("forums");

        getInitialForumsResponse.IsSuccessStatusCode.Should().BeTrue();

        var initialForums = await getInitialForumsResponse.Content.ReadFromJsonAsync<Forum[]>();
        initialForums
            .Should().NotBeNull().And
            .Subject.As<Forum[]>().Should().NotContain(f => f.Title.Equals(forumTitle));

        using var response = await httpClient.PostAsync("forums",
            JsonContent.Create(new { title = forumTitle }));
        response.Invoking(r => r.EnsureSuccessStatusCode()).Should().NotThrow();

        response.IsSuccessStatusCode.Should().BeTrue();

        var forum = await response.Content.ReadFromJsonAsync<Forum>();
        forum
            .Should().NotBeNull().And
            .Subject.As<Forum>().Title.Should().Be(forumTitle);

        using var getForumsResponse = await httpClient.GetAsync("forums");
        var forums = await getForumsResponse.Content.ReadFromJsonAsync<Forum[]>();
        forums
            .Should().NotBeNull().And
            .Subject.As<Forum[]>().Should().Contain(f => f.Title.Equals(forumTitle));
    }
}