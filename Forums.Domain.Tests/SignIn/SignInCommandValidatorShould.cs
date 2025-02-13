using FluentAssertions;
using Forums.Domain.UseCases.SignIn;

namespace Forums.Domain.Tests.SignIn;

public class SignInCommandValidatorShould
{
    private readonly SignInCommandValidator _sut;

    public SignInCommandValidatorShould()
    {
        _sut = new SignInCommandValidator();
    }

    [Fact]
    public void ReturnSuccess_WhenCommandValid()
    {
        var command = new SignInCommand("Test", "qwerty");
        _sut.Validate(command).IsValid.Should().BeTrue();
    }

    public static IEnumerable<object[]> GetInvalidCommands()
    {
        var command = new SignInCommand("Test", "qwerty");
        yield return new object[] { command with { Login = string.Empty } };
        yield return new object[] { command with { Login = "  " } };
        yield return new object[] { command with { Login = "123456789012345678901" } };
        yield return new object[] { command with { Password = "      " } };
        yield return new object[] { command with { Password = string.Empty } };
    }

    [Theory]
    [MemberData(nameof(GetInvalidCommands))]
    public void ReturnFailure_WhenCommandInvalid(SignInCommand command)
    {
        _sut.Validate(command).IsValid.Should().BeFalse();
    }
}