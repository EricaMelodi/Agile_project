using System.Text.Json;

namespace ElectricitySavingApp.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<ErrorHandlingMiddleware> logger;

    public ErrorHandlingMiddleware(
        RequestDelegate next,
        ILogger<ErrorHandlingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ArgumentException ex)
        {
            logger.LogWarning(ex, "Invalid request for {Path}.", context.Request.Path);
            await WriteProblemResponseAsync(
                context,
                StatusCodes.Status400BadRequest,
                "Invalid request.",
                "The request contains invalid input.");
        }
        catch (TaskCanceledException ex)
        {
            logger.LogWarning(ex, "External request timed out for {Path}.", context.Request.Path);
            await WriteProblemResponseAsync(
                context,
                StatusCodes.Status504GatewayTimeout,
                "External service timed out.",
                "An external service did not respond in time.");
        }
        catch (HttpRequestException ex)
        {
            logger.LogError(ex, "External request failed for {Path}.", context.Request.Path);
            await WriteProblemResponseAsync(
                context,
                StatusCodes.Status503ServiceUnavailable,
                "External service unavailable.",
                "An external service could not be reached.");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, "External service returned invalid JSON for {Path}.", context.Request.Path);
            await WriteProblemResponseAsync(
                context,
                StatusCodes.Status502BadGateway,
                "Invalid external response.",
                "An external service returned an invalid response.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception for {Path}.", context.Request.Path);
            await WriteProblemResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Internal server error.",
                "The server could not complete the request.");
        }
    }

    private static async Task WriteProblemResponseAsync(
        HttpContext context,
        int statusCode,
        string title,
        string detail)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsJsonAsync(new
        {
            type = $"https://httpstatuses.com/{statusCode}",
            title,
            status = statusCode,
            detail,
            instance = context.Request.Path.Value
        });
    }
}
