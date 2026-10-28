using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using JTradeERP.Application.Common.Exceptions;

namespace JTradeERP.Api.Infrastructure;

public sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger,
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var statusCode = exception switch
        {
            ValidationFailureException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            ConflictException => StatusCodes.Status409Conflict,
            JTradeERP.Application.Common.Exceptions.AuthenticationFailureException => StatusCodes.Status401Unauthorized,
            DbUpdateException { InnerException: SqlException { Number: 2601 or 2627 or 547 } } => StatusCodes.Status409Conflict,
            DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogInformation("Request validation failed. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }

        httpContext.Response.StatusCode = statusCode;

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = statusCode switch
            {
                StatusCodes.Status400BadRequest => "One or more validation errors occurred.",
                StatusCodes.Status404NotFound => "The requested resource was not found.",
                StatusCodes.Status409Conflict => "The request conflicts with the current resource state.",
                StatusCodes.Status401Unauthorized => "Authentication failed.",
                _ => "An unexpected error occurred."
            },
            Instance = httpContext.Request.Path,
            Extensions = { ["traceId"] = httpContext.TraceIdentifier }
        };

        if (exception is ValidationFailureException validationException)
        {
            problemDetails.Extensions["errors"] = validationException.Errors;
        }

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problemDetails,
            Exception = exception
        });

        return true;
    }
}
