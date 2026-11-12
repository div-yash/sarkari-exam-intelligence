using System.Text.Json;
using SarkariExamIntelligence.Application.Common.Exceptions;
using SarkariExamIntelligence.Application.Common.Models;

namespace SarkariExamIntelligence.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var statusCode = exception switch
        {
            AppException appEx => appEx.StatusCode,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode >= 500)
            _logger.LogError(exception, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning("Handled exception ({StatusCode}) on {Method} {Path}: {Message}",
                statusCode, context.Request.Method, context.Request.Path, exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        IReadOnlyList<string>? errors = exception is BadRequestException badReq && badReq.ValidationErrors.Count > 0
            ? badReq.ValidationErrors
            : null;

        var payload = ApiResponse<object>.Fail(
            statusCode >= 500 ? "An unexpected server error occurred. Please check logs." : exception.Message,
            errors);

        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
