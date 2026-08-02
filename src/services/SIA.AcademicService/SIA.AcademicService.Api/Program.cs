using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Infrastructure.MessageBus.Publishers;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.DataStores;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AcademicDbContext>(options =>
{
  var connectionString = builder.Configuration
      .GetConnectionString("AcademicDatabase");

  options.UseSqlServer(connectionString);
});

builder.Services.AddMassTransit(configurator =>
{
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
  });
});

builder.Services.AddHostedService<OutboxPublisherService>();

builder.Services.AddScoped<ISubjectDataStore, SubjectDataStore>();

builder.Services.AddScoped<CreateSubjectUseCase>();

builder.Services.AddScoped<IEducationalProgramsDataStore, EducationalProgramsDataStore>();

builder.Services.AddScoped<CreateEducationalProgramsUseCase>();

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
        service = "SIA.AcademicService.Api",
        status = "Healthy"
    });
});

app.Run();
