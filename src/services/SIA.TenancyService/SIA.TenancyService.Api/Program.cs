using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.TenancyService.Application.Interfaces.Queries;
using SIA.TenancyService.Application.UseCases.Tenants;
using SIA.TenancyService.Infrastructure.Persistence.Contexts;
using SIA.TenancyService.Infrastructure.Persistence.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TenancyDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("TenancyDatabase")
    ?? throw new InvalidOperationException("No se configuró ConnectionStrings:TenancyDatabase.");

  options.UseSqlServer(connectionString, sqlOptions =>
  {
    sqlOptions.EnableRetryOnFailure();
  });
});

builder.Services.AddScoped<ITenantQueries, TenantQueries>();
builder.Services.AddScoped<ResolveTenantUseCase>();

builder.Services.AddSiaExceptionHandling();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA TenancyService API v1");
  });
}

app.UseSiaExceptionHandling();
app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
  service = "SIA.TenancyService.Api",
  status = "Healthy"
}));

app.Run();
