using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using SIA.AdminBff.Infrastructure.Http;

namespace SIA.AdminBff.Infrastructure.Errors;

public sealed class BffExceptionHandler(ILogger<BffExceptionHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
  {
    var error = MapException(exception);
    var correlationId = httpContext.Items[CorrelationIdConstants.ItemKey] is Guid value ? value : Guid.NewGuid();

    if (error.StatusCode >= StatusCodes.Status500InternalServerError)
    {
      logger.LogError(exception, "La solicitud al AdminBff terminó con estado {StatusCode}. CorrelationId: {CorrelationId}.", error.StatusCode, correlationId);
    }
    else
    {
      logger.LogWarning(exception, "La solicitud al AdminBff terminó con estado {StatusCode}. CorrelationId: {CorrelationId}.", error.StatusCode, correlationId);
    }

    httpContext.Response.StatusCode = error.StatusCode;
    await httpContext.Response.WriteAsJsonAsync(new BffErrorResponse
    {
      Code = error.Code,
      Message = error.Message,
      CorrelationId = correlationId
    }, cancellationToken);

    return true;
  }

  private static BffErrorDescriptor MapException(Exception exception)
  {
    if (exception is UnauthorizedAccessException)
    {
      return new BffErrorDescriptor(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "La identidad de la sesión no es válida.");
    }

    if (exception is HttpRequestException)
    {
      return new BffErrorDescriptor(StatusCodes.Status503ServiceUnavailable, "SERVICE_UNAVAILABLE", "Uno de los servicios necesarios no está disponible.");
    }

    if (exception is InternalServiceException internalException)
    {
      return MapInternalServiceException(internalException.StatusCode);
    }

    return new BffErrorDescriptor(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Ocurrió un error inesperado.");
  }

  private static BffErrorDescriptor MapInternalServiceException(HttpStatusCode statusCode)
  {
    return statusCode switch
    {
      HttpStatusCode.BadRequest => new BffErrorDescriptor(StatusCodes.Status400BadRequest, "INVALID_REQUEST", "La solicitud no es válida."),
      HttpStatusCode.Unauthorized => new BffErrorDescriptor(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "La sesión no está autorizada."),
      HttpStatusCode.Forbidden => new BffErrorDescriptor(StatusCodes.Status403Forbidden, "FORBIDDEN", "No cuenta con permisos para realizar la operación."),
      HttpStatusCode.NotFound => new BffErrorDescriptor(StatusCodes.Status404NotFound, "RESOURCE_NOT_FOUND", "No se encontró el recurso solicitado."),
      HttpStatusCode.Conflict => new BffErrorDescriptor(StatusCodes.Status409Conflict, "CONFLICT", "La operación entra en conflicto con el estado actual del recurso."),
      _ => new BffErrorDescriptor(StatusCodes.Status502BadGateway, "INTERNAL_SERVICE_ERROR", "Un servicio interno no pudo completar la solicitud.")
    };
  }

  private sealed record BffErrorDescriptor(int StatusCode, string Code, string Message);
}
