using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SIA.BuildingBlocks.WebApi.ExceptionHandling;

public static class ExceptionHandlingExtensions
{
  public static IServiceCollection AddSiaExceptionHandling(this IServiceCollection services)
  {
    services.AddProblemDetails();
    services.AddExceptionHandler<GlobalExceptionHandler>();

    return services;
  }

  public static IApplicationBuilder UseSiaExceptionHandling(this IApplicationBuilder app)
  {
    app.UseExceptionHandler();

    return app;
  }
}
