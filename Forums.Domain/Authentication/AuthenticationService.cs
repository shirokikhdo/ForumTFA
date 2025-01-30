using Microsoft.Extensions.Options;

namespace Forums.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly ISymmetricDecryptor _decryptor;
    private readonly AuthenticationConfiguration _configuration;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IOptions<AuthenticationConfiguration> options)
    {
        _decryptor = decryptor;
        _configuration = options.Value;
    }

    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        var userIdString = await _decryptor.Decrypt(authToken, _configuration.Key, cancellationToken);
        return new User(Guid.Parse(userIdString));
    }
}