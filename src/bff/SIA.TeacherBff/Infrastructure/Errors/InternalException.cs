using System.Net;

namespace SIA.TeacherBff.Infrastructure.Errors;

public sealed class InternalException : Exception
{
  public InternalException(string service, HttpStatusCode statusCode)
      : base($"El servicio interno '{service}' respondió con estado HTTP {(int)statusCode}.")
  {
    Service = service;
    StatusCode = statusCode;
  }

  public string Service { get; }
  public HttpStatusCode StatusCode { get; }
}
