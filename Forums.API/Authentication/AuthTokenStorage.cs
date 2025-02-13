namespace Forums.API.Authentication;

internal class AuthTokenStorage : IAuthTokenStorage
{
    private const string HEADER_KEY = "ForumsTFA-Auth-Token";

    public bool TryExtract(HttpContext httpContext, out string token)
    {
        if (httpContext.Request.Cookies.TryGetValue(HEADER_KEY, out var value) 
            && !string.IsNullOrWhiteSpace(value))
        {
            token = value;
            return true;
        }
        token = string.Empty;
        return false;
    }

    public void Store(HttpContext httpContext, string token) =>
        httpContext.Response.Cookies.Append(HEADER_KEY, token);
}