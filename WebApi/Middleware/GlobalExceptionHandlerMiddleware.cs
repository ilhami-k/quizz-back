using System.Net;
using System.Text.Json;

namespace Api.Middleware;

public class GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        var (statusCode, message) = exception switch
        {
            KeyNotFoundException knfex => (HttpStatusCode.NotFound, knfex.Message),
            ArgumentException argex => (HttpStatusCode.BadRequest, argex.Message),
            _ => (HttpStatusCode.InternalServerError, "An internal server error has occurred. Please try again later.")
        };

        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(new
        {
            Message = message
        }));
    }
}
