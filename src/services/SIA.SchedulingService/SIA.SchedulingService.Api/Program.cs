using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.AcademicLoads;
using SIA.SchedulingService.Application.UseCases.AcademicOfferings;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;
using SIA.SchedulingService.Infrastructure.MessageBus.Publishers;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.DataStores;
using SIA.SchedulingService.Infrastructure.Persistence.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

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

builder.Services.AddScoped<IClassroomLabDataStore, ClassroomLabDataStore>();
builder.Services.AddScoped<IClassroomLabQueries, ClassroomLabQueries>();

builder.Services.AddScoped<IClassroomTypeDataStore, ClassroomTypeDataStore>();
builder.Services.AddScoped<IClassroomTypeQueries, ClassroomTypeQueries>();

// UseCases Buildings
builder.Services.AddScoped<CreateBuildingUseCase>();
builder.Services.AddScoped<UpdateBuildingUseCase>();
builder.Services.AddScoped<DeactivateBuildingUseCase>();
builder.Services.AddScoped<ActivateBuildingUseCase>();
builder.Services.AddScoped<GetBuildingByIdUseCase>();

// UseCases Groups
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

// UseCases: AcademicLoad
builder.Services.AddScoped<CreateAcademicLoadUseCase>();
builder.Services.AddScoped<UpdateAcademicLoadUseCase>();
builder.Services.AddScoped<DeactivateAcademicLoadUseCase>();
builder.Services.AddScoped<ActivateAcademicLoadUseCase>();
builder.Services.AddScoped<GetAcademicLoadByIdUseCase>();

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

app.UseSiaExceptionHandling();

app.Run();