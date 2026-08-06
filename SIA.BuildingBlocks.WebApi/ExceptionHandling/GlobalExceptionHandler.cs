using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SIA.BuildingBlocks.Application.Exceptions;

namespace SIA.BuildingBlocks.WebApi.ExceptionHandling;

public sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,  CancellationToken cancellationToken)
  {
    logger.LogError(exception, "Ocurrió una excepción al procesar la solicitud.");

    var problemDetails = exception switch
    {
      NotFoundException => CreateProblemDetails(
          StatusCodes.Status404NotFound,
          "Recurso no encontrado",
          exception.Message,
          "https://sia/errors/not-found"),

      ConflictException => CreateProblemDetails(
          StatusCodes.Status409Conflict,
          "Conflicto en la operación",
          exception.Message,
          "https://sia/errors/conflict"),

      ArgumentException => CreateProblemDetails(
          StatusCodes.Status400BadRequest,
          "Solicitud no válida",
          exception.Message,
          "https://sia/errors/invalid-request"),

      _ => CreateProblemDetails(
          StatusCodes.Status500InternalServerError,
          "Error interno del servidor",
          "Ocurrió un error inesperado.",
          "https://sia/errors/internal-server-error")
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

  private static ProblemDetails CreateProblemDetails(
      int status,
      string title,
      string detail,
      string type)
  {
    return new ProblemDetails
    {
      Status = status,
      Title = title,
      Detail = detail,
      Type = type
    };
  }
}
