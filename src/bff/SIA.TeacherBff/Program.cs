using SIA.TeacherBff.Extensions;
using SIA.TeacherBff.Infrastructure.Authentication;
using SIA.TeacherBff.Infrastructure.Errors;
using SIA.TeacherBff.Infrastructure.Http;
using SIA.TeacherBff.Infrastructure.OpenApi;
using SIA.TeacherBff.Infrastructure.Tenancy;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer<BearerSchemeTransformer>();
  options.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddAuth(builder.Configuration);
builder.Services.AddErrors();
builder.Services.AddScoped<ITenantContext, TenantContext>();
builder.Services.AddScoped<ICorrelationAccessor, CorrelationAccessor>();
builder.Services.AddClients(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA TeacherBff API v1");
  });
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationMiddleware>();
app.UseErrors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
{
  return Results.Ok(new
  {
    service = "SIA.TeacherBff",
    status = "Healthy"
  });
}).AllowAnonymous();

app.Run();

public partial class Program;
