namespace SIA.AdminBff.Infrastructure.Errors;

public static class InternalHttpResponseExtensions
{
  public static void EnsureInternalSuccess(this HttpResponseMessage response, string serviceName)
  {
    if (!response.IsSuccessStatusCode)
    {
      throw new InternalServiceException(serviceName, response.StatusCode);
    }
  }
}
