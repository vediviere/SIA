using Microsoft.EntityFrameworkCore;
using SIA.AcademicStaffService.Application.Interfaces.DataStores;
using SIA.AcademicStaffService.Application.Interfaces.Queries;
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

builder.Services.AddScoped<IProfessorDataStore, ProfessorDataStore>();
builder.Services.AddScoped<IProfessorQueries, ProfessorQueries>();

builder.Services.AddScoped<CreateProfessorUseCase>();
builder.Services.AddScoped<UpdateProfessorUseCase>();
builder.Services.AddScoped<ActivateProfessorUseCase>();
builder.Services.AddScoped<DeactivateProfessorUseCase>();

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