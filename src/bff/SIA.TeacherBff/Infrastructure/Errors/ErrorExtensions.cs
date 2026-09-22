namespace SIA.TeacherBff.Infrastructure.Errors;

public static class ErrorExtensions
{
  public static IServiceCollection AddErrors(this IServiceCollection services)
  {
    services.AddProblemDetails();
    services.AddExceptionHandler<ErrorHandler>();
    return services;
  }

  public static IApplicationBuilder UseErrors(this IApplicationBuilder app)
  {
    app.UseExceptionHandler();
    return app;
  }
}
