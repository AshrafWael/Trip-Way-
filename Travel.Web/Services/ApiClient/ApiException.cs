using System.Net;

namespace Travel.Web.Services.ApiClient;

/// <summary>
/// Wraps a non-success ApiResponse from Travel.API so MVC controllers can
/// catch one exception type and show the API's own message/errors instead
/// of a generic failure page.
/// </summary>
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public List<string> Errors { get; }

    public ApiException(HttpStatusCode statusCode, string message, List<string>? errors = null)
        : base(message)
    {
        StatusCode = statusCode;
        Errors = errors ?? new List<string>();
    }
}
