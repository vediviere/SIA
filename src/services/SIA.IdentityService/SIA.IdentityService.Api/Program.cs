using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SIA.BuildingBlocks.WebApi.ExceptionHandling;
using SIA.IdentityService.Api.OpenApi;
using SIA.IdentityService.Application.Interfaces.DataStores;
using SIA.IdentityService.Application.Interfaces.Security;
using SIA.IdentityService.Application.UseCases.Users;
using SIA.IdentityService.Infrastructure.MessageBus.Publishers;
using SIA.IdentityService.Infrastructure.Persistence.Contexts;
using SIA.IdentityService.Infrastructure.Persistence.DataStores;
using SIA.IdentityService.Infrastructure.Security;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddOpenApi(options =>
{
  options.AddDocumentTransformer<BearerSchemeTransformer>();
  options.AddOperationTransformer<AuthOperationTransformer>();
});

builder.Services.AddDbContext<IdentityDbContext>(options =>
{
  var connectionString = builder.Configuration.GetConnectionString("IdentityDatabase")
    ?? throw new InvalidOperationException("No se configuró ConnectionStrings:IdentityDatabase.");

  options.UseSqlServer(connectionString);
});

builder.Services.AddSiaExceptionHandling();

builder.Services.AddScoped<IUserDataStore, UserDataStore>();
builder.Services.AddScoped<IRoleDataStore, RoleDataStore>();
builder.Services.AddScoped<IPermissionDataStore, PermissionDataStore>();
builder.Services.AddScoped<IRefreshTokenDataStore, RefreshTokenDataStore>();

builder.Services.AddScoped<IPasswordHasher, PasswordHasherService>();
builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();

builder.Services.AddScoped<CreateStaffUserUseCase>();
builder.Services.AddScoped<ProvisionInitialAdministratorUseCase>();
builder.Services.AddScoped<SetInitialPasswordUseCase>();
builder.Services.AddScoped<LoginUseCase>();
builder.Services.AddScoped<RefreshUseCase>();
builder.Services.AddScoped<LogoutUseCase>();
builder.Services.AddScoped<AssignRoleUseCase>();
builder.Services.AddScoped<RevokeRoleUseCase>();
builder.Services.AddScoped<ChangeUserStatusUseCase>();
builder.Services.AddScoped<ChangePasswordUseCase>();

var signingKey = builder.Configuration["Token:SigningKey"]
  ?? throw new InvalidOperationException("Token:SigningKey no está configurado.");

var issuer = builder.Configuration["Token:Issuer"]
  ?? throw new InvalidOperationException("Token:Issuer no está configurado.");

var audience = builder.Configuration["Token:Audience"]
  ?? throw new InvalidOperationException("Token:Audience no está configurado.");

if (!int.TryParse(builder.Configuration["Token:ExpirationMinutes"], out var expirationMinutes) || expirationMinutes <= 0)
{
  throw new InvalidOperationException("Token:ExpirationMinutes no es válido.");
}

if (!int.TryParse(builder.Configuration["Token:RefreshTokenExpirationDays"], out var refreshTokenExpirationDays) || refreshTokenExpirationDays <= 0)
{
  throw new InvalidOperationException("Token:RefreshTokenExpirationDays no es válido.");
}

var signingKeyBytes = Convert.FromBase64String(signingKey);

if (signingKeyBytes.Length < 32)
{
  throw new InvalidOperationException("Token:SigningKey debe contener al menos 256 bits.");
}

var tokenSettings = new TokenSettings
{
  Issuer = issuer,
  Audience = audience,
  SigningKey = signingKey,
  ExpirationMinutes = expirationMinutes,
  RefreshTokenExpirationDays = refreshTokenExpirationDays
};

builder.Services.AddSingleton(tokenSettings);
builder.Services.AddScoped<ITokenGenerator, TokenGeneratorService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.MapInboundClaims = false;

  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = tokenSettings.Issuer,
    ValidAudience = tokenSettings.Audience,
    IssuerSigningKey = new SymmetricSecurityKey(signingKeyBytes),
    NameClaimType = "email",
    RoleClaimType = "role",
    ClockSkew = TimeSpan.FromSeconds(30)
  };
});

builder.Services.AddAuthorization(options =>
{
  options.AddPolicy("Users.Manage", policy => policy.RequireClaim("permission", "Users.Manage"));
});

builder.Services.AddMassTransit(configurator =>
{
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
  });
});

builder.Services.AddHostedService<OutboxPublisherService>();

var app = builder.Build();

if (args.Any(arg => string.Equals(arg, "--provision-admin", StringComparison.OrdinalIgnoreCase)))
{
  var tenantIdValue = app.Configuration["Provisioning:TenantId"];
  var email = app.Configuration["Provisioning:AdminEmail"];
  var temporaryPassword = app.Configuration["Provisioning:TemporaryPassword"];

  if (!Guid.TryParse(tenantIdValue, out var tenantId))
  {
    throw new InvalidOperationException("Provisioning:TenantId no contiene un Guid válido.");
  }

  if (string.IsNullOrWhiteSpace(email))
  {
    throw new InvalidOperationException("Provisioning:AdminEmail es obligatorio.");
  }

  if (string.IsNullOrWhiteSpace(temporaryPassword))
  {
    throw new InvalidOperationException("Provisioning:TemporaryPassword es obligatorio.");
  }

  using var scope = app.Services.CreateScope();

  var useCase = scope.ServiceProvider.GetRequiredService<ProvisionInitialAdministratorUseCase>();
  var result = await useCase.ExecuteAsync(tenantId, email, temporaryPassword, Guid.NewGuid(), CancellationToken.None);

  Console.WriteLine($"Administrador inicial provisionado correctamente. UserId: {result.Id} | TenantId: {result.TenantId} | Email: {result.Email}");

  return;
}

if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();

  app.UseSwaggerUI(options =>
  {
    options.SwaggerEndpoint("/openapi/v1.json", "SIA IdentityService API v1");
  });
}

app.UseSiaExceptionHandling();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok(new
{
  service = "SIA.IdentityService.Api",
  status = "Healthy"
}));

app.Run();
