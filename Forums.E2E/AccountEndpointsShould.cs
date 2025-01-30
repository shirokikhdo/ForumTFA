using System.Net.Http.Json;
using FluentAssertions;
using Forums.Domain.Authentication;
using Xunit.Abstractions;

namespace Forums.E2E;

public class AccountEndpointsShould : IClassFixture<ForumApiApplicationFactory>
{
    private readonly ForumApiApplicationFactory _factory;
    private readonly ITestOutputHelper _testOutputHelper;

    public AccountEndpointsShould(
        ForumApiApplicationFactory factory,
        ITestOutputHelper testOutputHelper)
    {
        _factory = factory;
        _testOutputHelper = testOutputHelper;
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
        signInResponse.Headers.Should().ContainKey("ForumsTFA-Auth-Token");
        
        _testOutputHelper.WriteLine(string.Join(Environment.NewLine,
            signInResponse.Headers.Select(h => $"{h.Key} = {string.Join(", ", h.Value)}")));
       
        var signedInUser = await signInResponse.Content.ReadFromJsonAsync<User>();
        signedInUser.Should()
            .NotBeNull().And
            .BeEquivalentTo(createdUser);
    }
}