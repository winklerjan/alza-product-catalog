using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ProductCatalog.Api;

/// <summary>
/// Maps domain invariant violations to a 400 Problem Details response.
/// Any other exception is left for the default exception handler to turn into a 500.
/// </summary>
public sealed partial class DomainExceptionHandler(ILogger<DomainExceptionHandler> logger) : IExceptionHandler
{
    /// <inheritdoc />
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ArgumentException)
        {
            return false;
        }

        LogDomainInvariantViolation(logger, exception);

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Invalid request",
            Detail = exception.Message,
        };

        httpContext.Response.StatusCode = problemDetails.Status.Value;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    [LoggerMessage(Level = LogLevel.Warning, Message = "Rejected request due to a domain invariant violation")]
    private static partial void LogDomainInvariantViolation(ILogger logger, Exception exception);
}
