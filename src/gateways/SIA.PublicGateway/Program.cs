using Microsoft.AspNetCore.RateLimiting;
using SIA.PublicGateway.Infrastructure.Authentication;
using SIA.PublicGateway.Infrastructure.Http;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddGatewayAuthentication(builder.Configuration);

var globalLimit = builder.Configuration.GetValue<int>("RateLimiting:Global:PermitLimit", 100);
var globalWindow = builder.Configuration.GetValue<int>("RateLimiting:Global:WindowSeconds", 60);
var identityLimit = builder.Configuration.GetValue<int>("RateLimiting:IdentityAuth:PermitLimit", 5);
var identityWindow = builder.Configuration.GetValue<int>("RateLimiting:IdentityAuth:WindowSeconds", 60);

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            return RateLimitPartition.GetNoLimiter("NoLimit");
        }

        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = globalLimit,
            Window = TimeSpan.FromSeconds(globalWindow),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    options.AddPolicy("IdentityAuthPolicy", context =>
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = identityLimit,
            Window = TimeSpan.FromSeconds(identityWindow),
            QueueLimit = 0
        });
    });
});

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
app.UseRateLimiter();

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

public partial class Program { }