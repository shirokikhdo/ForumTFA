using FluentAssertions;
using Forums.Domain.Authentication;

namespace Forums.Domain.Tests.Authentication;

public class PasswordManagerShould
{
    private readonly PasswordManager _sut;
    private static readonly byte[] EmptySalt = Enumerable.Repeat((byte)0, 100).ToArray();
    private static readonly byte[] EmptyHash = Enumerable.Repeat((byte)0, 32).ToArray();

    public PasswordManagerShould()
    {
        _sut = new PasswordManager();
    }

    [Theory]
    [InlineData("password")]
    [InlineData("qwerty123")]
    public void GenerateMeaningfulSaltAndHash(string password)
    {
        var (salt, hash) = _sut.GeneratePasswordParts(password);
        salt.Should().HaveCount(100).And.NotBeEquivalentTo(EmptySalt);
        hash.Should().HaveCount(32).And.NotBeEquivalentTo(EmptyHash);
    }

    [Fact]
    public void ReturnTrue_WhenPasswordMatch()
    {
        var password = "qwerty123";
        var (salt, hash) = _sut.GeneratePasswordParts(password);
        _sut.ComparePasswords(password, salt, hash).Should().BeTrue();
    }

    [Fact]
    public void ReturnFalse_WhenPasswordDoesntMatch()
    {
        var (salt, hash) = _sut.GeneratePasswordParts("qwerty123");
        _sut.ComparePasswords("p@s$w0rd", salt, hash).Should().BeFalse();
    }
}