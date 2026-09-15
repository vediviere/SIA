using SIA.PublicGateway.Infrastructure.Authentication;
using SIA.PublicGateway.Infrastructure.Http;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGatewayAuthentication(builder.Configuration);

builder.Services
  .AddReverseProxy()
  .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/admin-bff/openapi/v1.json", "SIA AdminBff vía PublicGateway v1");
  });
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapReverseProxy();

app.MapGet("/health", () =>
{
  return Results.Ok(new
  {
    service = "SIA.PublicGateway",
    status = "Healthy"
  });
});

app.Run();

public partial class Program;
