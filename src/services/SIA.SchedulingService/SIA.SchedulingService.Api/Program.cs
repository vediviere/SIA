using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SchedulingDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("SchedulingDatabase");

    options.UseSqlServer(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "SIA SchedulingService API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.SchedulingService.Api",
        status = "Healthy"
    });
});

app.Run();
