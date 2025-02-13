using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Forums.Domain.Authentication;

internal class AuthenticationService : IAuthenticationService
{
    private readonly ISymmetricDecryptor _decryptor;
    private readonly IAuthenticationStorage _storage;
    private readonly ILogger<AuthenticationService> _logger;
    private readonly AuthenticationConfiguration _configuration;

    public AuthenticationService(
        ISymmetricDecryptor decryptor,
        IAuthenticationStorage storage,
        ILogger<AuthenticationService> logger,
        IOptions<AuthenticationConfiguration> options)
    {
        _decryptor = decryptor;
        _storage = storage;
        _logger = logger;
        _configuration = options.Value;
    }

    public async Task<IIdentity> Authenticate(string authToken, CancellationToken cancellationToken)
    {
        string sessionIdString;
        try
        {
            sessionIdString = await _decryptor.Decrypt(authToken, _configuration.Key, cancellationToken);
        }
        catch (CryptographicException cryptographicException)
        {
            _logger.LogWarning(
                cryptographicException,
                "Cannot decrypt auth token, maybe someone is trying to forge it");
            
            return User.Guest;
        }

        if (!Guid.TryParse(sessionIdString, out var sessionId))
            return User.Guest;

        var session = await _storage.FindSession(sessionId, cancellationToken);
        if (session is null)
            return User.Guest;

        return session.ExpiresAt < DateTimeOffset.UtcNow 
            ? User.Guest 
            : new User(session.UserId, sessionId);
    }
}