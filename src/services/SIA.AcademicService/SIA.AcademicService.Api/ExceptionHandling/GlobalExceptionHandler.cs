using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SIA.AcademicService.Application.Common.Exceptions;

namespace SIA.AcademicService.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(
      HttpContext httpContext,
      Exception exception,
      CancellationToken cancellationToken)
  {
    logger.LogError(
        exception,
        "An exception occurred while processing the request.");

    var problemDetails = exception switch
    {
      NotFoundException => new ProblemDetails
      {
        Status = StatusCodes.Status404NotFound,
        Title = "Resource not found",
        Detail = exception.Message,
        Type = "https://sia/errors/not-found"
      },

      ArgumentException => new ProblemDetails
      {
        Status = StatusCodes.Status400BadRequest,
        Title = "Invalid request",
        Detail = exception.Message,
        Type = "https://sia/errors/invalid-request"
      },

      ConflictException => new ProblemDetails
      {
        Status = StatusCodes.Status409Conflict,
        Title = "Conflicto en la operación",
        Detail = exception.Message,
        Type = "https://sia/errors/conflict"
      },

      _ => new ProblemDetails
      {
        Status = StatusCodes.Status500InternalServerError,
        Title = "Internal server error",
        Detail = "An unexpected error occurred.",
        Type = "https://sia/errors/internal-server-error"
      }
    };

    problemDetails.Instance = httpContext.Request.Path;
    problemDetails.Extensions["traceId"] =
        httpContext.TraceIdentifier;

    httpContext.Response.StatusCode =
        problemDetails.Status
        ?? StatusCodes.Status500InternalServerError;

    await httpContext.Response.WriteAsJsonAsync(
        problemDetails,
        cancellationToken);

    return true;
  }
}
