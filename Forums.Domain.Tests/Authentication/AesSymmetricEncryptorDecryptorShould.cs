using Forums.Domain.Authentication;
using System.Security.Cryptography;
using FluentAssertions;

namespace Forums.Domain.Tests.Authentication;

public class AesSymmetricEncryptorDecryptorShould
{
    private readonly AesSymmetricEncryptorDecryptor _sut;

    public AesSymmetricEncryptorDecryptorShould()
    {
        _sut = new AesSymmetricEncryptorDecryptor();
    }

    [Fact]
    public async Task ReturnMeaningfulEncryptedString()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var actual = await _sut.Encrypt("Hello world!", key, CancellationToken.None);
        actual.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DecryptEncryptedString_WhenKeyIsSame()
    {
        var key = RandomNumberGenerator.GetBytes(32);
        var encrypted = await _sut.Encrypt("Hello world!", key, CancellationToken.None);
        var decrypted = await _sut.Decrypt(encrypted, key, CancellationToken.None);
        decrypted.Should().Be("Hello world!");
    }

    [Fact]
    public async Task ThrowException_WhenDecryptingWithDifferentKey()
    {
        var encrypted = await _sut.Encrypt(
            "Hello, world!", 
            RandomNumberGenerator.GetBytes(32), 
            CancellationToken.None);

        await _sut.Invoking(s => 
                s.Decrypt(encrypted, RandomNumberGenerator.GetBytes(32), CancellationToken.None))
            .Should().ThrowAsync<CryptographicException>();
    }
}