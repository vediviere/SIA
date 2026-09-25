using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Workflow;
using SIA.AdminBff.Controllers.Workflow;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Tests.Controllers.Workflow;

public sealed class WorkflowReviewsControllerTests
{
    [Fact]
    public async Task GetPendingReviewsAsync_WithValidTenant_ShouldReturnTrayList()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientFake(new List<ReviewProcessListItemDto>
        {
            new(
                Id: processId,
                TenantId: tenantId,
                Status: "InReview",
                CreatedAtUtc: DateTime.UtcNow
            )
        });





        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        var result = await controller.GetPendingReviewsAsync(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<ReviewProcessListItemDto>>(okResult.Value);

        var item = Assert.Single(response);
        Assert.Equal(processId, item.Id);
        Assert.Equal(tenantId, workflowClient.TenantId);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WithValidId_ShouldReturnDetail()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();

        var detailDto = new ReviewProcessDetailDto(
            Id: processId,
            TenantId: tenantId,
            Status: "InReview",
            CreatedAtUtc: DateTime.UtcNow
        );

        var workflowClient = new WorkflowClientFake(detailDto);
        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        var result = await controller.GetByIdAsync(processId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ReviewProcessDetailDto>(okResult.Value);

        Assert.Equal(processId, response.Id);
        Assert.Equal(tenantId, workflowClient.TenantId);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task ApproveAsync_WithValidId_ShouldReturnNoContent()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientFake();
        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        var result = await controller.ApproveAsync(processId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(tenantId, workflowClient.TenantId);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task ReturnAsync_WithValidRequest_ShouldReturnNoContent()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();
        var request = new ReturnRequestDto(new List<ObservationInputDto>());

        var workflowClient = new WorkflowClientFake();
        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        var result = await controller.ReturnAsync(processId, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(tenantId, workflowClient.TenantId);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WhenWorkflowServiceThrowsException_ShouldPropagateError()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientThrowingFake(new InvalidOperationException("Workflow service error"));
        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            controller.GetByIdAsync(processId, CancellationToken.None)
        );

        Assert.Equal("Workflow service error", exception.Message);
        Assert.Equal(tenantId, workflowClient.TenantId);
    }

    [Fact]
    public async Task ApproveAsync_WithCorrelationId_ShouldValidateCorrelationIdAndTenantPropagation()
    {
        var tenantId = Guid.NewGuid();
        var processId = Guid.NewGuid();
        var correlationId = Guid.NewGuid();

        var workflowClient = new WorkflowClientFake();
        var controller = new WorkflowReviewsController(workflowClient, new TenantContextFake(tenantId));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        controller.ControllerContext.HttpContext.TraceIdentifier = correlationId.ToString();

        var result = await controller.ApproveAsync(processId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(tenantId, workflowClient.TenantId);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    private sealed class TenantContextFake : ITenantContext
    {
        public TenantContextFake(Guid tenantId)
        {
            TenantId = tenantId;
        }

        public Guid TenantId { get; }
    }

    private sealed class WorkflowClientFake : IWorkflowClient
    {
        private readonly IEnumerable<ReviewProcessListItemDto> _listResponse;
        private readonly ReviewProcessDetailDto? _singleResponse;

        public WorkflowClientFake(IEnumerable<ReviewProcessListItemDto> listResponse)
        {
            _listResponse = listResponse;
            _singleResponse = null;
        }

        public WorkflowClientFake(ReviewProcessDetailDto singleResponse)
        {
            _listResponse = Enumerable.Empty<ReviewProcessListItemDto>();
            _singleResponse = singleResponse;
        }

        public WorkflowClientFake()
        {
            _listResponse = Enumerable.Empty<ReviewProcessListItemDto>();
            _singleResponse = null;
        }

        public Guid TenantId { get; private set; }
        public Guid ProcessId { get; private set; }

        public Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken)
        {
            TenantId = tenantId;
            return Task.FromResult(_listResponse);
        }

        public Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
        {
            TenantId = tenantId;
            ProcessId = processId;
            return Task.FromResult(_singleResponse);
        }

        public Task ApproveReviewAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
        {
            TenantId = tenantId;
            ProcessId = processId;
            return Task.CompletedTask;
        }

        public Task ReturnReviewAsync(Guid tenantId, Guid processId, ReturnRequestDto request, CancellationToken cancellationToken)
        {
            TenantId = tenantId;
            ProcessId = processId;
            return Task.CompletedTask;
        }
    }

    private sealed class WorkflowClientThrowingFake : IWorkflowClient
    {
        private readonly Exception _exceptionToThrow;
        public Guid TenantId { get; private set; }

        public WorkflowClientThrowingFake(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        public Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(Guid tenantId, CancellationToken cancellationToken) => throw _exceptionToThrow;

        public Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken)
        {
            TenantId = tenantId;
            throw _exceptionToThrow;
        }

        public Task ApproveReviewAsync(Guid tenantId, Guid processId, CancellationToken cancellationToken) => throw _exceptionToThrow;

        public Task ReturnReviewAsync(Guid tenantId, Guid processId, ReturnRequestDto request, CancellationToken cancellationToken) => throw _exceptionToThrow;
    }
}