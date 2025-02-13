using Forums.API.Authentication;
using Forums.Domain.Authentication;

namespace Forums.API.Middlewares;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public AuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext httpContext,
        IAuthTokenStorage tokenStorage,
        IAuthenticationService authenticationService,
        IIdentityProvider identityProvider)
    {
        var identity = tokenStorage.TryExtract(httpContext, out var authToken)
            ? await authenticationService.Authenticate(authToken, httpContext.RequestAborted)
            : User.Guest;
        identityProvider.Current = identity;
        await _next(httpContext);
    }
}