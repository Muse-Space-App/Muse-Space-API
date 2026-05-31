using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MuseSpace.Core.Results;

namespace MuseSpace.API.Middleware;

/// <summary>Middleware that catches unhandled exceptions and returns standardized error responses.</summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    /// <summary>Initializes a new instance of the <see cref="GlobalExceptionMiddleware"/> class.</summary>
    /// <param name="next">The next middleware in the request pipeline.</param>
    /// <param name="logger">The logger instance for recording exception details.</param>
    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>Invokes the middleware to process the HTTP request and catch any unhandled exceptions.</summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Default to 500
        var statusCode = (int)HttpStatusCode.InternalServerError;
        var message = "An internal server error has occurred.";

        // We can add specific exception types here later if needed (e.g. NotFoundException -> 404)

        context.Response.StatusCode = statusCode;

        var response = ApiResponse<object>.Fail(message, new List<string> { exception.Message });
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        await context.Response.WriteAsync(json);
    }
}
