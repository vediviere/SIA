using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.SchoolControlService.Api.OpenApi;
using SIA.SchoolControlService.Application.Interfaces;
using SIA.SchoolControlService.Application.Interfaces.DataStores;
using SIA.SchoolControlService.Application.Interfaces.Queries;
using SIA.SchoolControlService.Application.UseCases.Students;
using SIA.SchoolControlService.Application.UseCases.SubjectReferences;
using SIA.SchoolControlService.Infrastructure.MessageBus.Consumers;
using SIA.SchoolControlService.Infrastructure.Persistence.Contexts;
using SIA.SchoolControlService.Infrastructure.Persistence.DataStores;
using SIA.SchoolControlService.Infrastructure.Persistence.Queries;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer<BearerTransformer>();
  options.AddOperationTransformer<AuthTransformer>();
});

builder.Services.AddSiaExceptionHandling();

builder.Services.AddDbContext<SchoolControlDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("SchoolControlDatabase")
    ?? throw new InvalidOperationException("No se configuró ConnectionStrings:SchoolControlDatabase.");

  options.UseSqlServer(connectionString);
});

builder.Services.AddScoped<ISchoolControlDataStore, SchoolControlDataStore>();
builder.Services.AddScoped<IStudentDataStore, StudentDataStore>();
builder.Services.AddScoped<IStudentQueries, StudentQueries>();

builder.Services.AddScoped<GetSubjectReferenceUseCase>();
builder.Services.AddScoped<CreateUseCase>();
builder.Services.AddScoped<GetByNumberUseCase>();

var signingKey = builder.Configuration["Token:SigningKey"]
  ?? throw new InvalidOperationException("Token:SigningKey no está configurado.");

var issuer = builder.Configuration["Token:Issuer"]
  ?? throw new InvalidOperationException("Token:Issuer no está configurado.");

var audience = builder.Configuration["Token:Audience"]
  ?? throw new InvalidOperationException("Token:Audience no está configurado.");

var signingKeyBytes = Convert.FromBase64String(signingKey);

if (signingKeyBytes.Length < 32)
{
  throw new InvalidOperationException("Token:SigningKey debe contener al menos 256 bits.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.MapInboundClaims = false;

  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = issuer,
    ValidAudience = audience,
    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
    NameClaimType = "email",
    RoleClaimType = "role",
    ClockSkew = TimeSpan.FromSeconds(30)
  };
});

builder.Services.AddAuthorization();

builder.Services.AddMassTransit(configurator =>
{
  configurator.AddConsumer<SubjectCreatedConsumer>();

  configurator.UsingRabbitMq((context, rabbitMq) =>
  {
    var host = builder.Configuration["RabbitMq:Host"]
      ?? throw new InvalidOperationException("No se configuró RabbitMq:Host.");

    var virtualHost = builder.Configuration["RabbitMq:VirtualHost"]
      ?? throw new InvalidOperationException("No se configuró RabbitMq:VirtualHost.");

    var username = builder.Configuration["RabbitMq:Username"]
      ?? throw new InvalidOperationException("No se configuró RabbitMq:Username.");

    var password = builder.Configuration["RabbitMq:Password"]
      ?? throw new InvalidOperationException("No se configuró RabbitMq:Password.");

    rabbitMq.Host(host, virtualHost, hostConfigurator =>
    {
      hostConfigurator.Username(username);
      hostConfigurator.Password(password);
    });

    rabbitMq.ReceiveEndpoint("sia-school-control-subject-created-v1", endpoint =>
    {
      endpoint.ConfigureConsumer<SubjectCreatedConsumer>(context);
    });
  });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA SchoolControlService API v1");
  });
}

app.UseSiaExceptionHandling();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
  service = "SIA.SchoolControlService.Api",
  status = "Healthy"
}));

app.Run();
