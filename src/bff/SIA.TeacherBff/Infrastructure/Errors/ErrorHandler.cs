using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using SIA.TeacherBff.Infrastructure.Http;

namespace SIA.TeacherBff.Infrastructure.Errors;

public sealed class ErrorHandler(ILogger<ErrorHandler> logger) : IExceptionHandler
{
  public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
  {
    var error = Map(exception);
    var correlationId = context.Items[CorrelationConstants.ItemKey] is Guid value ? value : Guid.NewGuid();

    if (error.Status >= StatusCodes.Status500InternalServerError)
    {
      logger.LogError(exception, "La solicitud al TeacherBff terminó con estado {Status}. CorrelationId: {CorrelationId}.", error.Status, correlationId);
    }
    else
    {
      logger.LogWarning(exception, "La solicitud al TeacherBff terminó con estado {Status}. CorrelationId: {CorrelationId}.", error.Status, correlationId);
    }

    context.Response.StatusCode = error.Status;
    await context.Response.WriteAsJsonAsync(new ErrorResponse
    {
      Code = error.Code,
      Message = error.Message,
      CorrelationId = correlationId
    }, cancellationToken);

    return true;
  }

  private static ErrorDescriptor Map(Exception exception)
  {
    if (exception is UnauthorizedAccessException)
    {
      return new ErrorDescriptor(StatusCodes.Status401Unauthorized, "UNAUTHORIZED", "La identidad de la sesión no es válida.");
    }

    if (exception is HttpRequestException)
    {
      return new ErrorDescriptor(StatusCodes.Status503ServiceUnavailable, "SERVICE_UNAVAILABLE", "Uno de los servicios necesarios no está disponible.");
    }

    if (exception is InternalException internalException)
    {
      return MapInternal(internalException.StatusCode);
    }

    return new ErrorDescriptor(StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "Ocurrió un error inesperado.");
  }

  private static ErrorDescriptor MapInternal(HttpStatusCode status)
  {
    return status switch
    {
      HttpStatusCode.BadRequest => new ErrorDescriptor(400, "INVALID_REQUEST", "La solicitud no es válida."),
      HttpStatusCode.Unauthorized => new ErrorDescriptor(401, "UNAUTHORIZED", "La sesión no está autorizada."),
      HttpStatusCode.Forbidden => new ErrorDescriptor(403, "FORBIDDEN", "No cuenta con permisos para realizar la operación."),
      HttpStatusCode.NotFound => new ErrorDescriptor(404, "RESOURCE_NOT_FOUND", "No se encontró el recurso solicitado."),
      HttpStatusCode.Conflict => new ErrorDescriptor(409, "CONFLICT", "La operación entra en conflicto con el estado actual del recurso."),
      _ => new ErrorDescriptor(502, "INTERNAL_SERVICE_ERROR", "Un servicio interno no pudo completar la solicitud.")
    };
  }

  private sealed record ErrorDescriptor(int Status, string Code, string Message);
}
