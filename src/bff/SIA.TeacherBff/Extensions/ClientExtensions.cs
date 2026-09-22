using SIA.TeacherBff.Configuration;
using SIA.TeacherBff.Infrastructure.Http;

namespace SIA.TeacherBff.Extensions;

public static class ClientExtensions
{
  public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
  {
    services.AddTransient<RequestContextHandler>();

    AddClient(services, configuration, ServiceConfig.Academic);
    AddClient(services, configuration, ServiceConfig.Staff);
    AddClient(services, configuration, ServiceConfig.Scheduling);

    return services;
  }

  private static void AddClient(IServiceCollection services, IConfiguration configuration, string name)
  {
    var url = ServiceConfig.GetUrl(configuration, name);

    services.AddHttpClient(name, client =>
    {
      client.BaseAddress = url;
      client.Timeout = TimeSpan.FromSeconds(30);
    }).AddHttpMessageHandler<RequestContextHandler>();
  }
}
