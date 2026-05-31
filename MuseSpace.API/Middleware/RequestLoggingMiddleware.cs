namespace MuseSpace.API.Middleware;

/// <summary>
/// Middleware for logging incoming HTTP requests. This middleware captures the HTTP method and request path of each incoming request and logs this information using the provided ILogger instance. It is designed to be added to the application's request pipeline to ensure that all incoming requests are logged for monitoring and debugging purposes.
/// </summary>
public sealed class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    /// <summary>
    /// Constructor for RequestLoggingMiddleware
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    /// <remarks>
    /// This middleware is responsible for logging incoming HTTP requests.
    /// It captures the HTTP method and request path of each request and logs this information using the provided ILogger instance.
    /// </remarks>
    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Incoming request: {Method} {Path}", context.Request.Method, context.Request.Path);
        await _next(context);
    }
}

/// <summary>
/// Extension method to add the RequestLoggingMiddleware to the application's request pipeline.
/// </summary>
/// <remarks>
/// Integrates RequestLoggingMiddleware into the middleware pipeline.
/// </remarks>
public static class RequestLoggingMiddlewareExtensions
{
    /// <summary>
    /// Adds the RequestLoggingMiddleware to the application's request pipeline.
    /// This middleware will log the HTTP method and request path of each incoming request.
    /// </summary>
    /// <param name="app">The IApplicationBuilder instance to which the middleware will be added.</param>
    /// <returns>The IApplicationBuilder instance with the middleware added.</returns>
    /// <remarks>
    /// Ensure that all incoming HTTP requests are logged with their method and path.
    /// </remarks>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestLoggingMiddleware>();
    }
}
