using FluentAssertions;
using Forums.Domain.UseCases.SignOn;

namespace Forums.Domain.Tests.SignOn;

public class SignOnCommandValidatorShould
{
    private readonly SignOnCommandValidator _sut;

    public SignOnCommandValidatorShould()
    {
        _sut = new SignOnCommandValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var validCommand = new SignOnCommand("Test", "qwerty");
        _sut.Validate(validCommand).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var validCommand = new SignOnCommand("Test", "qwerty");
        yield return new object[] { validCommand with { Login = string.Empty } };
        yield return new object[] { validCommand with { Login = "123456789012345678901" } };
        yield return new object[] { validCommand with { Login = "   " } };
        yield return new object[] { validCommand with { Password = string.Empty } };
        yield return new object[] { validCommand with { Password = "     " } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandInvalid(SignOnCommand command)
    {
        _sut.Validate(command).IsValid.Should().BeFalse();
    }
}