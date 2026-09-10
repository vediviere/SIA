using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.Interfaces.DataStores;
using SIA.WorkflowService.Application.UseCases.ReviewProcesses;
using SIA.WorkflowService.Infrastructure.MessageBus.Consumers.Proposals;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.Infrastructure.Persistence.DataStores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("WorkflowDatabase")
      ?? throw new InvalidOperationException("No se configuró la conexión WorkflowDatabase.");

  options.UseSqlServer(connectionString, sqlOptions => sqlOptions.EnableRetryOnFailure());
});

builder.Services.AddScoped<IReviewStore, ReviewStore>();
builder.Services.AddScoped<CreateUseCase>();


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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA WorkflowService API v1");
  });
}


app.UseHttpsRedirection();

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
