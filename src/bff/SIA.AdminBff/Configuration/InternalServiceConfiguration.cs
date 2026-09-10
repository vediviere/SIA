namespace SIA.AdminBff.Configuration;

public static class InternalServiceConfiguration
{
  public const string AcademicService = "AcademicService";
  public const string SchedulingService = "SchedulingService";

  public static Uri GetRequiredBaseAddress(IConfiguration configuration, string serviceName)
  {
    var configurationKey = $"Services:{serviceName}:BaseUrl";
    var baseUrl = configuration[configurationKey];

    if (string.IsNullOrWhiteSpace(baseUrl) ||
        !Uri.TryCreate(baseUrl, UriKind.Absolute, out var baseAddress))
    {
      throw new InvalidOperationException($"{configurationKey} no contiene una URL absoluta válida.");
    }

    return baseAddress;
  }
}
