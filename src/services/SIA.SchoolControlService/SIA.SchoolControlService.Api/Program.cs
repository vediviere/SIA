using Microsoft.EntityFrameworkCore;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using MassTransit;
using SIA.SchoolControlService.Infrastructure.MessageBus.Consumers;
using SIA.SchoolControlService.Application.Interfaces;
using SIA.SchoolControlService.Application.UseCases.SubjectReferences;
using SIA.SchoolControlService.Infrastructure.Persistence.DataStores;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();



builder.Services.AddDbContext<SchoolControlDbContext>(options =>
{
  var connectionString = builder.Configuration
      .GetConnectionString("SchoolControlDatabase");

  options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ISchoolControlDataStore, SchoolControlDataStore>();

builder.Services.AddScoped<GetSubjectReferenceUseCase>();

builder.Services.AddMassTransit(configurator =>
{
  configurator.AddConsumer<SubjectCreatedConsumer>();

  configurator.UsingRabbitMq((context, rabbitMq) =>
  {
    var host = builder.Configuration["RabbitMq:Host"]
        ?? throw new InvalidOperationException(
            "No se configuró RabbitMq:Host.");

    var virtualHost = builder.Configuration["RabbitMq:VirtualHost"]
        ?? throw new InvalidOperationException(
            "No se configuró RabbitMq:VirtualHost.");

    var username = builder.Configuration["RabbitMq:Username"]
        ?? throw new InvalidOperationException(
            "No se configuró RabbitMq:Username.");

    var password = builder.Configuration["RabbitMq:Password"]
        ?? throw new InvalidOperationException(
            "No se configuró RabbitMq:Password.");

    rabbitMq.Host(
        host,
        virtualHost,
        hostConfigurator =>
        {
          hostConfigurator.Username(username);
          hostConfigurator.Password(password);
        });

    rabbitMq.ReceiveEndpoint(
        "sia-school-control-subject-created-v1",
        endpoint =>
        {
          endpoint.ConfigureConsumer<SubjectCreatedConsumer>(
                  context);
        });
  });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint(
        "/openapi/v1.json",
        "SIA SchoolControlService API v1");
  });
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.SchoolControlService.Api",
        status = "Healthy"
    });
});

app.Run();
