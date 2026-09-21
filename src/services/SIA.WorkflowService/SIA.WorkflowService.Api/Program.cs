using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.WorkflowService.Api.OpenApi;
using SIA.WorkflowService.Api.Security;
using SIA.WorkflowService.Application.Interfaces;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Infrastructure.MessageBus.Consumers.Proposals;
using SIA.WorkflowService.Infrastructure.MessageBus.Outbox;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer<BearerTransformer>();
    options.AddOperationTransformer<AuthTransformer>();
});

builder.Services.AddSiaExceptionHandling();

builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("WorkflowDatabase")
      ?? throw new InvalidOperationException("No se configuró la conexión WorkflowDatabase.");

    options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services.ConfigureOptions<JwtOptionsSetup>();

builder.Services.AddAuthorization();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

builder.Services.AddScoped<IReviewStore, ReviewStore>();
builder.Services.AddScoped<CreateUseCase>();
builder.Services.AddScoped<ApproveUseCase>();
builder.Services.AddScoped<ReturnUseCase>();

builder.Services.AddMassTransit(configurator =>
{
    configurator.AddConsumer<SubmittedConsumer>();

    configurator.UsingRabbitMq((context, rabbitMq) =>
    {
        var host = builder.Configuration["RabbitMq:Host"]
        ?? throw new InvalidOperationException("No se configuró RabbitMq:Host.");

        var virtualHost = builder.Configuration["RabbitMq:VirtualHost"]
        ?? throw new InvalidOperationException("No se configuró RabbitMq:VirtualHost.");

        var username = builder.Configuration["RabbitMq:Username"]
        ?? throw new InvalidOperationException("No se configuró RabbitMq:Username.");

        var password = builder.Configuration["RabbitMq:Password"]
        ?? throw new InvalidOperationException("No se configuró RabbitMq:Password.");

        rabbitMq.Host(host, virtualHost, hostConfigurator =>
      {
          hostConfigurator.Username(username);
          hostConfigurator.Password(password);
      });

        rabbitMq.ReceiveEndpoint("sia-workflow-proposal-submitted-v1", endpoint =>
      {
          endpoint.ConfigureConsumer<SubmittedConsumer>(context);
      });
    });
});

var outboxOptions = new OutboxOptions();
builder.Configuration.GetSection("Outbox").Bind(outboxOptions);

builder.Services.AddSingleton(outboxOptions);
builder.Services.AddSingleton(EventRegistry.Create());
builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddScoped<IOutboxEventPublisher, MassTransitOutboxEventPublisher>();
builder.Services.AddHostedService<OutboxPublisherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "SIA WorkflowService API v1");
    });
}

app.UseSiaExceptionHandling();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.WorkflowService.Api",
        status = "Healthy"
    });
});

app.Run();

public partial class Program;