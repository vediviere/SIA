using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIA.BuildingBlocks.Messaging.Inbox;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.ReportingService.Api.Middlewares;
using SIA.ReportingService.Api.OpenApi;
using SIA.ReportingService.Infrastructure.Persistence.Contexts;
using SIA.ReportingService.Infrastructure.Persistence.DataStores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Convert.FromBase64String(builder.Configuration["Token:SigningKey"]
                    ?? throw new InvalidOperationException("Falta Token.")))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerSchemeTransformer>();
    options.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddDbContext<ReportingDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("ReportingDatabase");
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    });
});

builder.Services.AddScoped<IInboxStore, InboxStore>();

builder.Services.AddMassTransit(configurator =>
{
    configurator.SetKebabCaseEndpointNameFormatter();

    configurator.UsingRabbitMq((context, rabbitMq) =>
    {
        var host = builder.Configuration["RabbitMq:Host"] ?? throw new InvalidOperationException("No se configuró RabbitMq:Host.");
        var virtualHost = builder.Configuration["RabbitMq:VirtualHost"] ?? throw new InvalidOperationException("No se configuró RabbitMq:VirtualHost.");
        var username = builder.Configuration["RabbitMq:Username"] ?? throw new InvalidOperationException("No se configuró RabbitMq:Username.");
        var password = builder.Configuration["RabbitMq:Password"] ?? throw new InvalidOperationException("No se configuró RabbitMq:Password.");

        rabbitMq.Host(host, virtualHost, hostConfigurator =>
        {
            hostConfigurator.Username(username);
            hostConfigurator.Password(password);
        });

        rabbitMq.UseConsumeFilter(typeof(InboxConsumeFilter<>), context);
        rabbitMq.ConfigureEndpoints(context);
    });
});

builder.Services.AddSiaExceptionHandling();

var app = builder.Build();

app.UseSiaExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SIA ReportingService API v1");
    });
}

app.UseHttpsRedirection();

app.UseMiddleware<CorrelationIdMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/health", async (ReportingDbContext dbContext, CancellationToken cancellationToken) =>
{
    var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);

    if (!canConnect)
    {
        return Results.Json(new
        {
            service = "SIA.ReportingService.Api",
            status = "Unhealthy"
        }, statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    return Results.Ok(new
    {
        service = "SIA.ReportingService.Api",
        status = "Healthy"
    });
}).AllowAnonymous();

app.Run();