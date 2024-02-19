using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

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
        var request = context.Request;
        request.EnableBuffering();

        var headers = string.Join(", ", request.Headers.Select(h => $"{h.Key}: {h.Value}"));
        var body = string.Empty;

        using (var reader = new StreamReader(request.Body, leaveOpen: true))
        {
            body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
        }

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request. Method: {Method}, Path: {Path}, Headers: {Headers}, Body: {Body}",
                context.Request.Method, context.Request.Path, headers, body);
            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync("An error occurred while processing your request. Please try again a bit later.");
        }
    }
}
