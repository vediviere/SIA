var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.AcademicService.Api",
        status = "Healthy"
    });
});

app.Run();
