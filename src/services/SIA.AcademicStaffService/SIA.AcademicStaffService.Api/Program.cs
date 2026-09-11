using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SIA.AcademicStaffService.Api.Security;
using SIA.AcademicStaffService.Application.Interfaces;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Application.UseCases.DivisionManagers;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Contracts.IntegrationEvents;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Coordinators;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.DivisionManagers;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Persons;
using SIA.AcademicStaffService.Contracts.IntegrationEvents.Professors;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;
using SIA.AcademicStaffService.Infrastructure.Persistence.Queries;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var signingKey = builder.Configuration["Token:SigningKey"]
    ?? throw new InvalidOperationException("Token:SigningKey no está configurado.");
var signingKeyBytes = Convert.FromBase64String(signingKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Token:Issuer"],
            ValidAudience = builder.Configuration["Token:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
            NameClaimType = "email",
            RoleClaimType = "role",
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

        document.Components.SecuritySchemes["Bearer"] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            Description = "Token JWT. Escribe el token directamente sin la palabra 'Bearer'."
        };

        document.Security ??= new List<OpenApiSecurityRequirement>();
        document.Security.Add(new OpenApiSecurityRequirement
        {
            { new OpenApiSecuritySchemeReference("Bearer", document), new List<string>() }
        });

        return Task.CompletedTask;
    });
});

builder.Services.AddDbContext<AcademicStaffDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("AcademicStaffDatabase");

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

var outboxOptions = new OutboxOptions();
builder.Configuration.GetSection("Outbox").Bind(outboxOptions);
builder.Services.AddSingleton(outboxOptions);

builder.Services.AddSingleton(new OutboxEventRegistry()
    .Register<TeacherCreatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.TeacherCreatedV1)
    .Register<TeacherUpdatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.TeacherUpdatedV1)
    .Register<TeacherActivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.TeacherActivatedV1)
    .Register<TeacherDeactivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.TeacherDeactivatedV1)
    .Register<DivisionHeadCreatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.DivisionHeadCreatedV1)
    .Register<DivisionHeadActivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.DivisionHeadActivatedV1)
    .Register<DivisionHeadDeactivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.DivisionHeadDeactivatedV1)
    .Register<CoordinatorCreatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.CoordinatorCreatedV1)
    .Register<CoordinatorActivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.CoordinatorActivatedV1)
    .Register<CoordinatorDeactivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.CoordinatorDeactivatedV1)
    .Register<PersonCreatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.PersonCreatedV1)
    .Register<PersonUpdatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.PersonUpdatedV1)
    .Register<PersonActivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.PersonActivatedV1)
    .Register<PersonDeactivatedIntegrationEvent>(AcademicStaffIntegrationEventTypes.PersonDeactivatedV1));

builder.Services.AddScoped<IOutboxStore, OutboxStore>();
builder.Services.AddScoped<IOutboxEventPublisher, MassTransitOutboxEventPublisher>();
builder.Services.AddHostedService<OutboxPublisherService>();

builder.Services.AddScoped<ITeacherDataStore, TeacherDataStore>();
builder.Services.AddScoped<ITeacherQueries, TeacherQueries>();
builder.Services.AddScoped<CreateTeacherUseCase>();
builder.Services.AddScoped<UpdateTeacherUseCase>();
builder.Services.AddScoped<ActivateTeacherUseCase>();
builder.Services.AddScoped<DeactivateTeacherUseCase>();

builder.Services.AddScoped<IDivisionHeadDataStore, DivisionHeadDataStore>();
builder.Services.AddScoped<IDivisionHeadQueries, DivisionHeadQueries>();
builder.Services.AddScoped<CreateDivisionHeadUseCase>();
builder.Services.AddScoped<ActivateDivisionHeadUseCase>();
builder.Services.AddScoped<DeactivateDivisionHeadUseCase>();

builder.Services.AddScoped<IPersonDataStore, PersonDataStore>();
builder.Services.AddScoped<IPersonQueries, PersonQueries>();
builder.Services.AddScoped<CreatePersonUseCase>();
builder.Services.AddScoped<UpdatePersonUseCase>();
builder.Services.AddScoped<ActivatePersonUseCase>();
builder.Services.AddScoped<DeactivatePersonUseCase>();

builder.Services.AddScoped<ICoordinatorDataStore, CoordinatorDataStore>();
builder.Services.AddScoped<ICoordinatorQueries, CoordinatorQueries>();
builder.Services.AddScoped<CreateCoordinatorUseCase>();
builder.Services.AddScoped<ActivateCoordinatorUseCase>();
builder.Services.AddScoped<DeactivateCoordinatorUseCase>();

builder.Services.AddSiaExceptionHandling();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ITenantContext, TenantContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "SIA.AcademicStaffService.Api");
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.AcademicStaffService.Api",
        status = "Healthy"
    });
}).AllowAnonymous();

app.UseSiaExceptionHandling();

app.Run();