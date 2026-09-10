using SIA.AdminBff.Extensions;
using SIA.AdminBff.Infrastructure.Authentication;
using SIA.AdminBff.Infrastructure.Errors;
using SIA.AdminBff.Infrastructure.Http;
using SIA.AdminBff.Infrastructure.OpenApi;
using SIA.AdminBff.Infrastructure.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer<BearerSchemeTransformer>();
  options.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddBffAuthentication(builder.Configuration);
builder.Services.AddBffExceptionHandling();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
builder.Services.AddInternalClients(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA AdminBff API v1");
  });
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseBffExceptionHandling();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
{
  return Results.Ok(new
  {
    service = "SIA.AdminBff",
    status = "Healthy"
  });
});

app.Run();
