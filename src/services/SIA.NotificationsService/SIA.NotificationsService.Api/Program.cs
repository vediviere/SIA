var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.NotificationsService.Api",
        status = "Healthy"
    });
});

app.Run();
