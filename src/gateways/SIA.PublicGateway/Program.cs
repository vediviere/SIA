var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

app.UseHttpsRedirection();

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
