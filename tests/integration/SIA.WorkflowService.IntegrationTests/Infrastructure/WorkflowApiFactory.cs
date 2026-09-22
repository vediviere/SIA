using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using SIA.BuildingBlocks.Messaging.Outbox;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;

namespace SIA.WorkflowService.IntegrationTests.Infrastructure;

public sealed class WorkflowApiFactory : WebApplicationFactory<Program>
{
    public const string TestSigningKey = "MDEyMzQ1Njc4OUFCQ0RFRjAxMjM0NTY3ODlBQkNERUY=";

    private readonly SqliteConnection _connection = new("Data Source=:memory:");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((context, configuration) =>
        {
            configuration.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Token:Issuer"] = "SIA.IdentityService",
                    ["Token:Audience"] = "SIA.Platform",
                    ["Token:SigningKey"] = TestSigningKey
                });
        });

        builder.ConfigureServices(services =>
        {
            ReplaceDatabase(services);
            RemoveExternalHostedServices(services);
        });
    }

    private void ReplaceDatabase(IServiceCollection services)
    {
        services.RemoveAll<DbContextOptions<WorkflowDbContext>>();
        services.RemoveAll<IDbContextOptionsConfiguration<WorkflowDbContext>>();

        if (_connection.State != System.Data.ConnectionState.Open)
        {
            _connection.Open();
        }

        services.AddDbContext<WorkflowDbContext>(options =>
        {
            options.UseSqlite(_connection);
        });
    }

    private static void RemoveExternalHostedServices(IServiceCollection services)
    {
        var descriptors = services
            .Where(descriptor =>
                descriptor.ServiceType == typeof(IHostedService) &&
                descriptor.ImplementationType == typeof(OutboxPublisherService))
            .ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }

        services.RemoveMassTransitHostedService();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection.Dispose();
        }
    }

    public async Task InitializeDatabaseAsync()
    {
        using var scope = CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        await dbContext.Database.EnsureCreatedAsync();
    }

    public IServiceScope CreateScope()
    {
        return Services.CreateScope();
    }
}