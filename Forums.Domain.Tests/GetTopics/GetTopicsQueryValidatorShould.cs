using FluentAssertions;
using Forums.Domain.UseCases.GetTopics;

namespace Forums.Domain.Tests.GetTopics;

public class GetTopicsQueryValidatorShould
{
    private readonly GetTopicsQueryValidator _sut;

    public GetTopicsQueryValidatorShould()
    {
        _sut = new GetTopicsQueryValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenQueryIsValid()
    {
        var query = new GetTopicsQuery(
            Guid.Parse("DA60E33E-7F32-4BFC-A4FF-E19F9BFE934B"),
            10,
            5);
        _sut.Validate(query).IsValid.Should().BeTrue();
    }
    public static IEnumerable<object[]> GetInvalidQuery()
    {
        var query = new GetTopicsQuery(Guid.Parse("DA60E33E-7F32-4BFC-A4FF-E19F9BFE934B"), 10, 5);
        yield return new object[] { query with { ForumId = Guid.Empty } };
        yield return new object[] { query with { Skip = -40 } };
        yield return new object[] { query with { Take = -1 } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidQuery))]
    public void ReturnFailure_WhenQueryIsInvalid(GetTopicsQuery query) =>
        _sut.Validate(query).IsValid.Should().BeFalse();
}