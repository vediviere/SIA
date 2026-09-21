using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIA.IdentityService.Contracts.Enums;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;

namespace SIA.WorkflowService.IntegrationTests.ReviewProcesses;

public sealed class ReviewProcessesTests : IClassFixture<WorkflowApiFactory>, IAsyncLifetime
{
    private readonly WorkflowApiFactory _factory;

    public ReviewProcessesTests(WorkflowApiFactory factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _factory.InitializeDatabaseAsync();
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }


    [Fact]
    public async Task Health_WithoutAuthentication_ShouldReturnOk()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Approve_WithoutAuthentication_ShouldReturnUnauthorized()
    {
        // Arrange
        var client = _factory.CreateClient();
        var processId = Guid.NewGuid();

        // Act
        var response = await client.PostAsync(
            $"/api/review-processes/{processId}/approve",
            content: null);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Approve_WithValidTenantToken_ShouldReturnNoContent()
    {
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var process = new ReviewProcess(
            tenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        using (var scope = _factory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

            dbContext.ReviewProcesses.Add(process);
            await dbContext.SaveChangesAsync();
        }

        var token = JwtTokenFactory.Create(
            userId,
            tenantId,
            nameof(RoleCode.Coordinator));

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Add(
            "X-Correlation-Id",
            correlationId.ToString());

        var response = await client.PostAsync(
            $"/api/review-processes/{process.Id}/approve",
            content: null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        using var assertScope = _factory.CreateScope();

        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var persistedProcess = await assertDbContext.ReviewProcesses
            .AsNoTracking()
            .SingleAsync(x => x.Id == process.Id);

        Assert.Equal(ReviewStatus.Approved, persistedProcess.Status);
        Assert.Equal(userId, persistedProcess.DecidedBy);
        Assert.Equal(correlationId, persistedProcess.DecisionCorrelationId);

        var outboxMessage = await assertDbContext.OutboxMessages
            .AsNoTracking()
            .SingleAsync(x => x.CorrelationId == correlationId);

        Assert.Equal(
            ReviewEventTypes.ProposalApprovedV1,
            outboxMessage.EventType);
    }

    [Fact]
    public async Task Approve_WithDifferentTenantToken_ShouldReturnNotFound()
    {
        // Arrange
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var process = new ReviewProcess(
            tenantBId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        using (var scope = _factory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

            dbContext.ReviewProcesses.Add(process);
            await dbContext.SaveChangesAsync();
        }

        var token = JwtTokenFactory.Create(
            userId,
            tenantAId,
            nameof(RoleCode.Coordinator));

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        client.DefaultRequestHeaders.Add(
            "X-Correlation-Id",
            correlationId.ToString());

        // Act
        var response = await client.PostAsync(
            $"/api/review-processes/{process.Id}/approve",
            content: null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);

        using var assertScope = _factory.CreateScope();

        var assertDbContext = assertScope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        var persistedProcess = await assertDbContext.ReviewProcesses
            .AsNoTracking()
            .SingleAsync(x => x.Id == process.Id);

        Assert.Equal(ReviewStatus.InReview, persistedProcess.Status);
        Assert.Null(persistedProcess.DecidedBy);
        Assert.Null(persistedProcess.DecidedAtUtc);
        Assert.Null(persistedProcess.DecisionCorrelationId);

        var outboxMessageExists = await assertDbContext.OutboxMessages
            .AsNoTracking()
            .AnyAsync(x => x.CorrelationId == correlationId);

        Assert.False(outboxMessageExists);
    }
}