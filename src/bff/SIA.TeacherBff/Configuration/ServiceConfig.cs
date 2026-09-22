namespace SIA.TeacherBff.Configuration;

public static class ServiceConfig
{
  public const string Academic = "AcademicService";
  public const string Staff = "AcademicStaffService";
  public const string Scheduling = "SchedulingService";

  public static Uri GetUrl(IConfiguration configuration, string name)
  {
    var key = $"Services:{name}:BaseUrl";
    var value = configuration[key];

    if (string.IsNullOrWhiteSpace(value) || !Uri.TryCreate(value, UriKind.Absolute, out var url))
    {
      throw new InvalidOperationException($"{key} no contiene una URL absoluta válida.");
    }

    return url;
  }
}
