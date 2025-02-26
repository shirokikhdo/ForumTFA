using FluentAssertions;
using Forums.Domain.Models;
using Forums.Domain.UseCases.GetForums;
using Moq;
using Moq.Language.Flow;

namespace Forums.Domain.Tests.GetForums;

public class GetForumsUseCaseShould
{
    private readonly GetForumsUseCase _sut;
    private readonly ISetup<IGetForumsStorage, Task<IEnumerable<Forum>>> _getForumsSetup;
    private readonly Mock<IGetForumsStorage> _storage;

    public GetForumsUseCaseShould()
    {
        _storage = new Mock<IGetForumsStorage>();
        _getForumsSetup = _storage.Setup(s => 
            s.GetForums(It.IsAny<CancellationToken>()));
        _sut = new GetForumsUseCase(_storage.Object);
    }

    [Fact]
    public async Task ReturnForums_FromStorage()
    {
        var forums = new Forum[]
        {
            new() { Id = Guid.Parse("EC04826E-314F-49D9-95ED-56D165D5DF21"), Title = "Test forum 1" },
            new() { Id = Guid.Parse("BFFFBA1A-405F-4B6D-B207-67013A8CC695"), Title = "Test forum 2" }
        };
        _getForumsSetup.ReturnsAsync(forums);
        var query = new GetForumsQuery();
        var actual = await _sut.Handle(query, CancellationToken.None);
        actual.Should().BeSameAs(forums);
        
        _storage.Verify(s => s.GetForums(CancellationToken.None), Times.Once);
        _storage.VerifyNoOtherCalls();
    }
}