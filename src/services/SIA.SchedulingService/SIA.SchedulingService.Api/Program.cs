using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.Classrooms;
using SIA.SchedulingService.Application.UseCases.ClassroomTypes;
using SIA.SchedulingService.Infrastructure.MessageBus.Publishers;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.DataStores;
using SIA.SchedulingService.Infrastructure.Persistence.Queries;

var builder = WebApplication.CreateBuilder(args); 

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<SchedulingDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("SchedulingDatabase");

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

// DataStores y Queries
builder.Services.AddScoped<IClassroomLabDataStore, ClassroomLabDataStore>();
builder.Services.AddScoped<IClassroomLabQueries, ClassroomLabQueries>();

builder.Services.AddScoped<IClassroomTypeDataStore, ClassroomTypeDataStore>();
builder.Services.AddScoped<IClassroomTypeQueries, ClassroomTypeQueries>();

// UseCases ClassroomTypes
builder.Services.AddScoped<CreateClassroomTypeUseCase>();
builder.Services.AddScoped<UpdateClassroomTypeUseCase>();
builder.Services.AddScoped<SoftDeleteClassroomTypeUseCase>();
builder.Services.AddScoped<RestoreClassroomTypeUseCase>();

// UseCases ClassroomLabs
builder.Services.AddScoped<CreateClassroomLabUseCase>();
builder.Services.AddScoped<UpdateClassroomLabUseCase>();
builder.Services.AddScoped<SoftDeleteClassroomLabUseCase>();
builder.Services.AddScoped<RestoreClassroomLabUseCase>();

builder.Services.AddSiaExceptionHandling();

var app = builder.Build(); 

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
