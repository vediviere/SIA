using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Application.UseCases.ClassSchedules;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Application.UseCases.SupportActivities;
using SIA.SchedulingService.Application.UseCases.SupportSchedules;
using SIA.SchedulingService.Application.UseCases.TeachingSupportHours;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Contracts.IntegrationEvents.Building;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassSchedule;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportActivity;
using SIA.SchedulingService.Contracts.IntegrationEvents.SupportSchedules;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.DataStores;
using SIA.SchedulingService.Infrastructure.Persistence.Queries;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
  .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddSiaExceptionHandling();

// Configuración de base de datos
builder.Services.AddDbContext<SchedulingDbContext>(options =>
{
  var connectionString = builder.Configuration
      .GetConnectionString("SchedulingDatabase");

  options.UseSqlServer(connectionString);
});

// Configuración de RabbitMQ (Bus de mensajes)
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

var outboxOptions = new OutboxOptions();
builder.Configuration.GetSection("Outbox").Bind(outboxOptions);
builder.Services.AddSingleton(outboxOptions);

builder.Services.AddSingleton(new OutboxEventRegistry()
    .Register<AcademicLoadDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadDeactivatedV1)
    .Register<AcademicLoadActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadActivatedV1)

    .Register<AcademicOfferingCreatedIntegrationEvet>(SchedulingIntegrationEventTypes.AcademicOfferingCreatedV1)
    .Register<AcademicOfferingUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingStatusUpdatedV1)
    .Register<AcademicOfferingDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingDeactivatedV1)
    .Register<AcademicOfferingActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicOfferingActivatedV1)

    .Register<BuildingCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingCreatedV1)
    .Register<BuildingUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingUpdatedV1)
    .Register<BuildingDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingDeactivatedV1)
    .Register<BuildingActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.BuildingActivatedV1)

    .Register<GroupCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupCreatedV1)
    .Register<GroupUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupUpdatedV1)
    .Register<GroupDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.GroupDeactivatedV1)
    .Register<GroupActivateIntegrationEvent>(SchedulingIntegrationEventTypes.GroupActivatedV1)

    .Register<TeachingSupportHoursCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursCreatedV1)
    .Register<TeachingSupportHoursUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursUpdatedV1)
    .Register<TeachingSupportHoursDeactivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursDeactivatedV1)
    .Register<TeachingSupportHoursActivatedIntegrationEvent>(SchedulingIntegrationEventTypes.TeachingSupportHoursActivatedV1)

    // SupportActivity
    .Register<SupportActivityCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityCreatedV1)
    .Register<SupportActivityUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityUpdatedV1)
    .Register<SupportActivityDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityDeletedV1)
    .Register<SupportActivityRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.SupportActivityRestoredV1)

    // ClassSchedule
    .Register<ClassScheduleCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleCreatedV1)
    .Register<ClassScheduleUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleUpdatedV1)
    .Register<ClassScheduleDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleDeletedV1)
    .Register<ClassScheduleRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassScheduleRestoredV1)

    // SupportSchedule
    .Register<SupportScheduleCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleCreatedV1)
    .Register<SupportScheduleUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleUpdatedV1)
    .Register<SupportScheduleDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleDeletedV1)
    .Register<SupportScheduleRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.SupportScheduleRestoredV1)

    // ClassroomLab
    .Register<ClassroomLabCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabCreatedV1)
    .Register<ClassroomLabUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabUpdatedV1)
    .Register<ClassroomLabDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabDeletedV1)
    .Register<ClassroomLabRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomLabRestoredV1)

    // ClassroomType
    .Register<ClassroomTypeCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeCreatedV1)
    .Register<ClassroomTypeUpdatedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeUpdatedV1)
    .Register<ClassroomTypeDeletedIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeDeletedV1)
    .Register<ClassroomTypeRestoredIntegrationEvent>(SchedulingIntegrationEventTypes.ClassroomTypeRestoredV1)

    // Proposal
    .Register<ProposalCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.ProposalCreatedV1)
    .Register<AcademicLoadCreatedIntegrationEvent>(SchedulingIntegrationEventTypes.AcademicLoadCreatedV1)

    .Register<ProposalSubmittedForReviewIntegrationEvent>(SchedulingIntegrationEventTypes.ProposalSubmittedForReviewV1)
);

builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddScoped<IOutboxEventPublisher, MassTransitOutboxEventPublisher>();
builder.Services.AddHostedService<OutboxPublisherService>();

// DataStores y Queries
builder.Services.AddScoped<IBuildingDataStore, BuildingDataStore>();
builder.Services.AddScoped<IBuildingQueries, BuildingQueries>();

builder.Services.AddScoped<IGroupDataStore, GroupDataStore>();
builder.Services.AddScoped<IGroupQueries, GroupQueries>();

builder.Services.AddScoped<IAcademicLoadDataStore, AcademicLoadDataStore>();
builder.Services.AddScoped<IAcademicLoadQueries, AcademicLoadQueries>();

builder.Services.AddScoped<IAcademicOfferingDataStore, AcademicOfferingDataStore>();
builder.Services.AddScoped<IAcademicOfferingQueries, AcademicOfferingQueries>();

builder.Services.AddScoped<ITeachingSupportHoursDataStore, TeachingSupportHoursDataStore>();
builder.Services.AddScoped<ITeachingSupportHoursQueries, TeachingSupportHoursQueries>();

builder.Services.AddScoped<IClassroomLabDataStore, ClassroomLabDataStore>();
builder.Services.AddScoped<IClassroomLabQueries, ClassroomLabQueries>();

builder.Services.AddScoped<IClassroomTypeDataStore, ClassroomTypeDataStore>();
builder.Services.AddScoped<IClassroomTypeQueries, ClassroomTypeQueries>();

builder.Services.AddScoped<ISupportScheduleDataStore, SupportScheduleDataStore>();
builder.Services.AddScoped<ISupportScheduleQueries, SupportScheduleQueries>();

builder.Services.AddScoped<IClassScheduleDataStore, ClassScheduleDataStore>();
builder.Services.AddScoped<IClassScheduleQueries, ClassScheduleQueries>();

builder.Services.AddScoped<ISupportActivityDataStore, SupportActivityDataStore>();
builder.Services.AddScoped<ISupportActivityQueries, SupportActivityQueries>();

builder.Services.AddScoped<AcademicLoadClassHoursCalculator>();
builder.Services.AddScoped<AcademicLoadSupportHoursCalculator>();
builder.Services.AddScoped<IProposalDataStore, ProposalDataStore>();
builder.Services.AddScoped<ProposalValidator>();


// UseCases: Buildings
builder.Services.AddScoped<CreateBuildingUseCase>();
builder.Services.AddScoped<UpdateBuildingUseCase>();
builder.Services.AddScoped<DeactivateBuildingUseCase>();
builder.Services.AddScoped<ActivateBuildingUseCase>();
builder.Services.AddScoped<GetBuildingByIdUseCase>();

// UseCases: Groups
builder.Services.AddScoped<CreateGroupUseCase>();
builder.Services.AddScoped<UpdateGroupUseCase>();
builder.Services.AddScoped<DeactivateGroupUseCase>();
builder.Services.AddScoped<ActivateGroupUseCase>();
builder.Services.AddScoped<GetGroupByIdUseCase>();

//UseCases: AcademicOffering
builder.Services.AddScoped<CreateAcademicOfferingUseCase>();
builder.Services.AddScoped<UpdateAcademicOfferingUseCase>();
builder.Services.AddScoped<DeactivateAcademicOfferingUseCase>();
builder.Services.AddScoped<ActivateAcademicOfferingUseCase>();
builder.Services.AddScoped<GetAcademicOfferingByIdUseCase>();

// UseCases: AcademicLoad
builder.Services.AddScoped<CreateAcademicLoadUseCase>();
builder.Services.AddScoped<UpdateAcademicLoadUseCase>();
builder.Services.AddScoped<DeactivateAcademicLoadUseCase>();
builder.Services.AddScoped<ActivateAcademicLoadUseCase>();
builder.Services.AddScoped<GetAcademicLoadByIdUseCase>();

//TeachingSupportHours
builder.Services.AddScoped<CreateTeachingSupportHoursUseCase>();
builder.Services.AddScoped<UpdateTeachingSupportHoursUseCase>();
builder.Services.AddScoped<DeactivateTeachingSupportHoursUseCase>();
builder.Services.AddScoped<ActivateTeachingSupportHoursUseCase>();
builder.Services.AddScoped<GetTeachingSupportHoursByIdUseCase>();

// UseCases: ClassroomTypes
builder.Services.AddScoped<CreateClassroomTypeUseCase>();
builder.Services.AddScoped<UpdateClassroomTypeUseCase>();
builder.Services.AddScoped<SoftDeleteClassroomTypeUseCase>();
builder.Services.AddScoped<RestoreClassroomTypeUseCase>();

// UseCases: ClassroomLabs
builder.Services.AddScoped<CreateClassroomLabUseCase>();
builder.Services.AddScoped<UpdateClassroomLabUseCase>();
builder.Services.AddScoped<SoftDeleteClassroomLabUseCase>();
builder.Services.AddScoped<RestoreClassroomLabUseCase>();

// UseCases: SupportSchedules
builder.Services.AddScoped<CreateSupportScheduleUseCase>();
builder.Services.AddScoped<UpdateSupportScheduleUseCase>();
builder.Services.AddScoped<SoftDeleteSupportScheduleUseCase>();
builder.Services.AddScoped<RestoreSupportScheduleUseCase>();

// UseCases: ClassSchedules
builder.Services.AddScoped<CreateClassScheduleUseCase>();
builder.Services.AddScoped<UpdateClassScheduleUseCase>();
builder.Services.AddScoped<SoftDeleteClassScheduleUseCase>();
builder.Services.AddScoped<RestoreClassScheduleUseCase>();

// UseCases: SupportActivity
builder.Services.AddScoped<CreateSupportActivityUseCase>();
builder.Services.AddScoped<UpdateSupportActivityUseCase>();
builder.Services.AddScoped<SoftDeleteSupportActivityUseCase>();
builder.Services.AddScoped<RestoreSupportActivityUseCase>();

// UseCases: AcademicLoadProposals
builder.Services.AddScoped<CreateProposalUseCase>();
builder.Services.AddScoped<SubmitProposalForReviewUseCase>();

var app = builder.Build();

app.UseSiaExceptionHandling();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint(
          "/openapi/v1.json",
          "SIA SchedulingService API v1");
  });
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
  return Results.Ok(new
  {
    service = "SIA.SchedulingService.Api",
    status = "Healthy"
  });
});

//app.UseSiaExceptionHandling();

app.Run();
