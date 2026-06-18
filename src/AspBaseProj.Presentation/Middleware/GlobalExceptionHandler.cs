using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace AspBaseProj.Presentation.Middleware;

/// <summary>
/// Global exception handler that formats errors as Problem Details (RFC 7807).
/// </summary>
public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var traceId = httpContext.TraceIdentifier;

        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.io/500",
            Title = "An unexpected error occurred",
            Status = (int)HttpStatusCode.InternalServerError,
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = traceId }
        };

        switch (exception)
        {
            case ValidationException validationEx:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Validation failed";
                problemDetails.Type = "https://httpstatuses.io/400";
                problemDetails.Extensions["errors"] = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            case KeyNotFoundException:
                problemDetails.Status = (int)HttpStatusCode.NotFound;
                problemDetails.Title = "Resource not found";
                problemDetails.Type = "https://httpstatuses.io/404";
                problemDetails.Detail = exception.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case UnauthorizedAccessException:
                problemDetails.Status = (int)HttpStatusCode.Forbidden;
                problemDetails.Title = "Access denied";
                problemDetails.Type = "https://httpstatuses.io/403";
                problemDetails.Detail = exception.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                break;

            case InvalidOperationException:
                problemDetails.Status = (int)HttpStatusCode.BadRequest;
                problemDetails.Title = "Invalid operation";
                problemDetails.Type = "https://httpstatuses.io/400";
                problemDetails.Detail = exception.Message;
                httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                _logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", traceId);
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                break;
        }

        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
