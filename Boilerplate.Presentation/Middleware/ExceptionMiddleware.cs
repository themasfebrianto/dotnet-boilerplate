using System.Diagnostics;
using System.Text.Json;
using Boilerplate.Application.Common.Exceptions;

namespace Boilerplate.Presentation.Middleware;

/// <summary>
/// Global exception handling middleware.
/// Maps AppException types to HTTP responses automatically.
/// </summary>
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            // Expected business exception - log as warning
            logger.LogWarning("Business exception: {ErrorCode} - {Message}", ex.ErrorCode, ex.Message);
            await WriteErrorResponse(context, ex.StatusCode, ex.Message, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            // Unexpected exception - log as error with trace
            var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
            logger.LogError(ex, "Unhandled exception. TraceId: {TraceId}", traceId);
            await WriteErrorResponse(context, 500, "An unexpected error occurred.", "ServerError", traceId);
        }
    }

    private static async Task WriteErrorResponse(
        HttpContext context,
        int statusCode,
        string message,
        string errorCode,
        string? traceId = null)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            success = false,
            message,
            errorCode,
            traceId
        };

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}

/// <summary>
/// Extension method for registering the exception middleware.
/// </summary>
public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionMiddleware>();
}
