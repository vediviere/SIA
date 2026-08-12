using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Application.Interfaces.Queries;
using SIA.SchedulingService.Application.UseCases.Buildings;
using SIA.SchedulingService.Infrastructure.Persistence.Contexts;
using SIA.SchedulingService.Infrastructure.Persistence.DataStores;
using SIA.SchedulingService.Infrastructure.Persistence.Queries;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.SchedulingService.Infrastructure.MessageBus.Publishers;
using SIA.SchedulingService.Application.UseCases.Groups;
using SIA.SchedulingService.Contracts.IntegrationEvents.Group;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddSiaExceptionHandling();


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

builder.Services.AddScoped<IBuildingDataStore, BuildingDataStore>();
builder.Services.AddScoped<IGroupDataStore, GroupDataStore>();

builder.Services.AddScoped<IBuildingQueries, BuildingQueries>();
builder.Services.AddScoped<IGroupQueries, GroupQueries>();

builder.Services.AddScoped<CreateBuildingUseCase>();
builder.Services.AddScoped<UpdateBuildingUseCase>();
builder.Services.AddScoped<DeactivateBuildingUseCase>();
builder.Services.AddScoped<ActivateBuildingUseCase>();
builder.Services.AddScoped<GetBuildingByIdUseCase>();

builder.Services.AddScoped<CreateGroupUseCase>();
builder.Services.AddScoped<UpdateGroupUseCase>();
builder.Services.AddScoped<DeactivateGroupUseCase>();
builder.Services.AddScoped<ActivateGroupUseCase>();
builder.Services.AddScoped<GetGroupByIdUseCase>();

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


app.Run();
