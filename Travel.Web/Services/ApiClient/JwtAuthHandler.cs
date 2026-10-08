using System.Net.Http.Headers;
using System.Security.Claims;

namespace Travel.Web.Services.ApiClient;

/// <summary>
/// Attaches the JWT that was issued by Travel.API at login time -- stored
/// as a claim on the MVC auth cookie -- to every outgoing request to the
/// API, so Travel.Web never needs to ask the person to log in twice.
/// </summary>
public class JwtAuthHandler : DelegatingHandler
{
    public const string TokenClaimType = "access_token";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtAuthHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.User
            .FindFirst(TokenClaimType)?.Value;

        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        return base.SendAsync(request, cancellationToken);
    }
}
