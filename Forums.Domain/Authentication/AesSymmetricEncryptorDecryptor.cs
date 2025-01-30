using System.Security.Cryptography;
using System.Text;

namespace Forums.Domain.Authentication;

internal class AesSymmetricEncryptorDecryptor : ISymmetricEncryptor, ISymmetricDecryptor
{
    private const int IV_SIZE = 16;

    private readonly Lazy<Aes> _aes;

    public AesSymmetricEncryptorDecryptor()
    {
        _aes = new Lazy<Aes>(Aes.Create);
    }

    public async Task<string> Encrypt(string plainText, byte[] key, CancellationToken cancellationToken)
    {
        var iv = RandomNumberGenerator.GetBytes(IV_SIZE);
        using var encryptedStream = new MemoryStream();
        await encryptedStream.WriteAsync(iv, cancellationToken);
        var encryptor = _aes.Value.CreateEncryptor(key, iv);
        await using (var stream = new CryptoStream(encryptedStream, encryptor, CryptoStreamMode.Write))
        {
            await stream.WriteAsync(Encoding.UTF8.GetBytes(plainText), cancellationToken);
        }
        return Convert.ToBase64String(encryptedStream.ToArray());
    }

    public async Task<string> Decrypt(string encryptedText, byte[] key, CancellationToken cancellationToken)
    {
        var encryptedBytes = Convert.FromBase64String(encryptedText);
        var iv = new byte[IV_SIZE];
        Array.Copy(encryptedBytes, 0, iv, 0, IV_SIZE);
        using var decryptedStream = new MemoryStream();
        var decryptor = _aes.Value.CreateDecryptor(key, iv);
        await using (var stream = new CryptoStream(decryptedStream, decryptor, CryptoStreamMode.Write))
        {
            await stream.WriteAsync(encryptedBytes.AsMemory(IV_SIZE), cancellationToken);
        }
        return Encoding.UTF8.GetString(decryptedStream.ToArray());
    }
}