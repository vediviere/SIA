using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SIA.IdentityService.Contracts.Enums;
using SIA.WorkflowService.Contracts.IntegrationEvents.ReviewProcesses;
using SIA.WorkflowService.Contracts.Responses.ReviewProcesses;
using SIA.WorkflowService.Domain.Entities;
using SIA.WorkflowService.Domain.Enums;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using SIA.WorkflowService.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

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

    [Fact]
    public async Task GetInReview_WithValidTenantToken_ShouldReturnOnlyInReviewProcessesForTenant()
    {
        // Arrange
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenantAInReview = new ReviewProcess(
            tenantAId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        var tenantAApproved = new ReviewProcess(
            tenantAId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        tenantAApproved.Approve(
            Guid.NewGuid(),
            DateTime.UtcNow,
            Guid.NewGuid());

        var tenantBInReview = new ReviewProcess(
            tenantBId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        using (var scope = _factory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

            dbContext.ReviewProcesses.AddRange(
                tenantAInReview,
                tenantAApproved,
                tenantBInReview);

            await dbContext.SaveChangesAsync();
        }

        var token = JwtTokenFactory.Create(
            userId,
            tenantAId,
            nameof(RoleCode.Coordinator));

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync("/api/review-processes");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var processes = await response.Content
                .ReadFromJsonAsync<List<ReviewProcessListItemResponse>>();

        Assert.NotNull(processes);

        var returnedProcess = Assert.Single(processes);

        Assert.Equal(tenantAInReview.Id, returnedProcess.Id);

        Assert.DoesNotContain(processes, process => process.Id == tenantAApproved.Id);

        Assert.DoesNotContain(processes, process => process.Id == tenantBInReview.Id);
    }

    [Fact]
    public async Task GetById_WithValidTenantToken_ShouldReturnProcess()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

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

        // Act
        var response = await client.GetAsync($"/api/review-processes/{process.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content
                .ReadFromJsonAsync<ReviewProcessResponse>();

        Assert.NotNull(result);

        Assert.Equal(process.Id, result.Id);
        Assert.Equal(process.AcademicLoadProposalId, result.AcademicLoadProposalId);
        Assert.Equal(process.DivisionHeadId, result.DivisionHeadId);
        Assert.Equal(process.Version, result.Version);
        Assert.Equal((int)process.Status, result.Status);
    }

    [Fact]
    public async Task GetById_WithDifferentTenantToken_ShouldReturnNotFound()
    {
        // Arrange
        var tenantAId = Guid.NewGuid();
        var tenantBId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var tenantBProcess = new ReviewProcess(
            tenantBId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            1,
            DateTime.UtcNow,
            Guid.NewGuid());

        using (var scope = _factory.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

            dbContext.ReviewProcesses.Add(tenantBProcess);

            await dbContext.SaveChangesAsync();
        }

        var token = JwtTokenFactory.Create(
            userId,
            tenantAId,
            nameof(RoleCode.Coordinator));

        var client = _factory.CreateClient();

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync($"/api/review-processes/{tenantBProcess.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}