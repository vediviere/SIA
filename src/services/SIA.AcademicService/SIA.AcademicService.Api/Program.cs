using MassTransit;
using Microsoft.EntityFrameworkCore;
using SIA.AcademicService.Application.Interfaces.DataStores;
using SIA.AcademicService.Application.Interfaces.Queries;
using SIA.AcademicService.Application.UseCases.AcademicPeriods;
using SIA.AcademicService.Application.UseCases.EducationalProgramsUseCase;
using SIA.AcademicService.Application.UseCases.StudyPlans;
//using SIA.AcademicService.Application.UseCases.StudyPlanSubjects;
using SIA.AcademicService.Application.UseCases.Subjects;
using SIA.AcademicService.Infrastructure.MessageBus.Publishers;
using SIA.AcademicService.Infrastructure.Persistence.Contexts;
using SIA.AcademicService.Infrastructure.Persistence.DataStores;
using SIA.AcademicService.Infrastructure.Persistence.Queries;



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

builder.Services.AddScoped<IEducationalProgramsQueries, EducationalProgramsQueries>();

builder.Services.AddScoped<UpdateEducationalProgramsUseCase>();

builder.Services.AddScoped<DeactivateEducationalProgramsUseCase>();

builder.Services.AddScoped<RestoreEducationalProgramsUseCase>();

builder.Services.AddScoped<IStudyPlanDataStore, StudyPlanDataStore>();

builder.Services.AddScoped<IStudyPlanQueries, StudyPlanQueries>();

builder.Services.AddScoped<CreateStudyPlanUseCase>();

builder.Services.AddScoped<UpdateStudyPlanUseCase>();

builder.Services.AddScoped<DeactivateStudyPlanUseCase>();

builder.Services.AddScoped<RestoreStudyPlanUseCase>();


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

app.Run();
