namespace SIA.TeacherBff.Infrastructure.Errors;

public static class ResponseExtensions
{
  public static void EnsureInternalSuccess(this HttpResponseMessage response, string service)
  {
    if (!response.IsSuccessStatusCode)
    {
      throw new InternalException(service, response.StatusCode);
    }
  }
}
