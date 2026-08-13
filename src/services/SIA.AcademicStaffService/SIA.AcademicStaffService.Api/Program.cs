using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
using SIA.AcademicStaffService.Application.UseCases.Coordinators;
using SIA.AcademicStaffService.Application.UseCases.DivisionManagers;
using SIA.AcademicStaffService.Application.UseCases.Persons;
using SIA.AcademicStaffService.Application.UseCases.Professors;
using SIA.AcademicStaffService.Infrastructure.Persistence.Contexts;
using SIA.AcademicStaffService.Infrastructure.Persistence.DataStores;
using SIA.AcademicStaffService.Infrastructure.Persistence.Queries;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();

builder.Services.AddDbContext<AcademicStaffDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("AcademicStaffDatabase");

    options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ITeacherDataStore, TeacherDataStore>();
builder.Services.AddScoped<ITeacherQueries, TeacherQueries>();
builder.Services.AddScoped<CreateTeacherUseCase>();
builder.Services.AddScoped<UpdateTeacherUseCase>();
builder.Services.AddScoped<ActivateTeacherUseCase>();
builder.Services.AddScoped<DeactivateTeacherUseCase>();

builder.Services.AddScoped<IDivisionHeadDataStore, DivisionHeadDataStore>();
builder.Services.AddScoped<IDivisionHeadQueries, DivisionHeadQueries>();
builder.Services.AddScoped<CreateDivisionHeadUseCase>();
builder.Services.AddScoped<UpdateDivisionHeadUseCase>();
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
builder.Services.AddScoped<UpdateCoordinatorUseCase>();
builder.Services.AddScoped<ActivateCoordinatorUseCase>();
builder.Services.AddScoped<DeactivateCoordinatorUseCase>();

builder.Services.AddSiaExceptionHandling();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "SIA AcademicStaffService API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapGet("/health", () =>
{
    return Results.Ok(new
    {
        service = "SIA.AcademicStaffService.Api",
        status = "Healthy"
    });
});

app.UseSiaExceptionHandling();

app.Run();