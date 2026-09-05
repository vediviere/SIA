namespace SIA.AdminBff.Infrastructure.Errors;

public static class ErrorHandlingExtensions
{
  public static IServiceCollection AddBffExceptionHandling(this IServiceCollection services)
  {
    services.AddProblemDetails();
    services.AddExceptionHandler<BffExceptionHandler>();
    return services;
  }

  public static IApplicationBuilder UseBffExceptionHandling(this IApplicationBuilder app)
  {
    app.UseExceptionHandler();
    return app;
  }
}
