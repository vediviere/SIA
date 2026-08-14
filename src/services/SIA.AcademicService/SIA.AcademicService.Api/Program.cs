using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Application.UseCases.StudyPlans;
//using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.DataStores;
using SIA.AcademicService.Infrastructure.Persistence.Queries;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.AcademicService.Contracts.IntegrationEvents;
using SIA.AcademicService.Contracts.IntegrationEvents.Subjects;
using SIA.AcademicService.Contracts.IntegrationEvents.AcademicPeriods;
using SIA.AcademicService.Contracts.IntegrationEvents.EducationalPrograms;
using SIA.AcademicService.Contracts.IntegrationEvents.StudyPlans;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AcademicDbContext>(options =>
{
  var connectionString = builder.Configuration
      .GetConnectionString("AcademicDatabase");

  options.UseSqlServer(connectionString, sqlOptions =>
  {
    sqlOptions.EnableRetryOnFailure();
  });

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

builder.Services.AddSingleton(new OutboxOptions());

builder.Services.AddSingleton(new OutboxEventRegistry()
  .Register<SubjectCreatedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectCreatedV1)
  .Register<SubjectUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectUpdatedV1)
  .Register<SubjectDeletedIntegrationEvent>(AcademicIntegrationEventTypes.SubjectDeletedV1)
  .Register<SubjectRestoredIntegrationEvent>(AcademicIntegrationEventTypes.SubjectRestoredV1)
  .Register<AcademicPeriodCreatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodCreatedV1)
  .Register<AcademicPeriodUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodUpdatedV1)
  .Register<AcademicPeriodDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodDeactivatedV1)
  .Register<AcademicPeriodActivatedIntegrationEvent>(AcademicIntegrationEventTypes.AcademicPeriodActivatedV1)
  .Register<EducationalProgramCreatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramCreatedV1)
  .Register<EducationalProgramUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramUpdatedV1)
  .Register<EducationalProgramDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramDeactivatedV1)
  .Register<EducationalProgramRestoredIntegrationEvent>(AcademicIntegrationEventTypes.EducationalProgramRestoredV1)
  .Register<StudyPlanCreatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanCreatedV1)
  .Register<StudyPlanUpdatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanUpdatedV1)
  .Register<StudyPlanDeactivatedIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanDeactivatedV1)
  .Register<StudyPlanRestoredIntegrationEvent>(AcademicIntegrationEventTypes.StudyPlanRestoredV1));

builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddScoped<IOutboxEventPublisher, MassTransitOutboxEventPublisher>();
builder.Services.AddHostedService<OutboxPublisherService>();

builder.Services.AddScoped<ISubjectDataStore, SubjectDataStore>();


builder.Services.AddScoped<IAcademicPeriodsDataStore, AcademicPeriodsDataStore>();
builder.Services.AddScoped<IAcademicPeriodQueries, AcademicPeriodQueries>();

builder.Services.AddScoped<CreateAcademicPeriodsUseCase>();
builder.Services.AddScoped<UpdateAcademicPeriodUseCase>();
builder.Services.AddScoped<PatchAcademicPeriodUseCase>();
builder.Services.AddScoped<DeactivateAcademicPeriodUseCase>();
builder.Services.AddScoped<ActivateAcademicPeriodUseCase>();
builder.Services.AddScoped<SearchAcademicPeriodsUseCase>();
builder.Services.AddScoped<GetAcademicPeriodByIdUseCase>();



//  DataStores y Queries
builder.Services.AddScoped<ISubjectDataStore, SubjectDataStore>();
builder.Services.AddScoped<ISubjectQueries, SubjectQueries>();

// DataStores y Queries de los Planes de Estudio
//builder.Services.AddScoped<IStudyPlanSubjectDataStore, StudyPlanSubjectDataStore>();
//builder.Services.AddScoped<IStudyPlanQueries, StudyPlanQueries>();

// UseCases Subjects
builder.Services.AddScoped<CreateSubjectUseCase>();
builder.Services.AddScoped<UpdateSubjectUseCase>();
builder.Services.AddScoped<SoftDeleteSubjectUseCase>();
builder.Services.AddScoped<RestoreSubjectUseCase>();

// UseCases de StudyPlanSubjects
//builder.Services.AddScoped<CreateStudyPlanSubjectUseCase>();
//builder.Services.AddScoped<UpdateStudyPlanSubjectUseCase>();
//builder.Services.AddScoped<DeleteStudyPlanSubjectUseCase>();
//builder.Services.AddScoped<RestoreStudyPlanSubjectUseCase>();

builder.Services.AddScoped<CreateSubjectUseCase>();

builder.Services.AddScoped<IEducationalProgramDataStore, EducationalProgramsDataStore>();

builder.Services.AddScoped<CreateEducationalProgramsUseCase>();

builder.Services.AddScoped<IEducationalProgramQueries, EducationalProgramQueries>();

builder.Services.AddScoped<UpdateEducationalProgramsUseCase>();

builder.Services.AddScoped<DeactivateEducationalProgramsUseCase>();

builder.Services.AddScoped<RestoreEducationalProgramsUseCase>();

builder.Services.AddScoped<IStudyPlanDataStore, StudyPlanDataStore>();

builder.Services.AddScoped<IStudyPlanQueries, StudyPlanQueries>();

builder.Services.AddScoped<CreateStudyPlanUseCase>();

builder.Services.AddScoped<UpdateStudyPlanUseCase>();

builder.Services.AddScoped<DeactivateStudyPlanUseCase>();

builder.Services.AddScoped<RestoreStudyPlanUseCase>();

builder.Services.AddSiaExceptionHandling();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint(
        "/openapi/v1.json",
        "SIA AcademicService API v1");
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

app.UseSiaExceptionHandling();

app.Run();
