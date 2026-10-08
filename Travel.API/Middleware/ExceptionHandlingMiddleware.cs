using System.Net;
using System.Text.Json;
using Travel.BLL.DTOs.Common;
using Travel.BLL.Exceptions;

namespace Travel.API.Middleware;

/// <summary>
/// Single place that turns any exception into the platform-wide
/// ApiResponse envelope with the right status code, so controllers never
/// need their own try/catch for these cases.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await HandleAsync(context, exception);
        }
    }

    private async Task HandleAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, exception.Message),
            AuthenticationFailedException => (HttpStatusCode.Unauthorized, exception.Message),
            BusinessRuleException => (HttpStatusCode.BadRequest, exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, "You do not have permission to perform this action."),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred.")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            // Only unexpected exceptions are logged with full detail and
            // hidden from the client -- the others are expected business
            // outcomes, not bugs, so their message is safe to return.
            _logger.LogError(exception, "Unhandled exception processing {Method} {Path}",
                context.Request.Method, context.Request.Path);
        }

        var response = ApiResponse<object>.Fail(message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
