using System.Net;

namespace SIA.AdminBff.Infrastructure.Errors;

public sealed class InternalServiceException : Exception
{
  public InternalServiceException(string serviceName, HttpStatusCode statusCode)
      : base($"El servicio interno '{serviceName}' respondió con estado HTTP {(int)statusCode}.")
  {
    ServiceName = serviceName;
    StatusCode = statusCode;
  }

  public string ServiceName { get; }
  public HttpStatusCode StatusCode { get; }
}
