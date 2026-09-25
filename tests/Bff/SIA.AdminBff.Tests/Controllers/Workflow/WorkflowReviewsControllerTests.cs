using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Workflow;
using SIA.AdminBff.Controllers.Workflow;

namespace SIA.AdminBff.Tests.Controllers.Workflow;

public sealed class WorkflowReviewsControllerTests
{
    [Fact]
    public async Task GetPendingReviewsAsync_ShouldReturnTrayList()
    {
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientFake(new List<ReviewProcessListItemDto>
        {
            new(
                Id: processId,
                Status: "InReview",
                CreatedAtUtc: DateTime.UtcNow
            )
        });

        var controller = new AdminReviewProcessesController(workflowClient);

        var result = await controller.GetPendingReviewsAsync(CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsAssignableFrom<IEnumerable<ReviewProcessListItemDto>>(okResult.Value);

        var item = Assert.Single(response);
        Assert.Equal(processId, item.Id);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WithValidId_ShouldReturnDetail()
    {
        var processId = Guid.NewGuid();

        var detailDto = new ReviewProcessDetailDto(
            Id: processId,
            Status: "InReview",
            CreatedAtUtc: DateTime.UtcNow
        );

        var workflowClient = new WorkflowClientFake(detailDto);
        var controller = new AdminReviewProcessesController(workflowClient);

        var result = await controller.GetByIdAsync(processId, CancellationToken.None);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ReviewProcessDetailDto>(okResult.Value);

        Assert.Equal(processId, response.Id);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task ApproveAsync_WithValidId_ShouldReturnNoContent()
    {
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientFake();
        var controller = new AdminReviewProcessesController(workflowClient);

        var result = await controller.ApproveAsync(processId, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task ReturnAsync_WithValidRequest_ShouldReturnNoContent()
    {
        var processId = Guid.NewGuid();
        var request = new ReturnRequestDto(new List<ObservationInputDto>());

        var workflowClient = new WorkflowClientFake();
        var controller = new AdminReviewProcessesController(workflowClient);

        var result = await controller.ReturnAsync(processId, request, CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(processId, workflowClient.ProcessId);
    }

    [Fact]
    public async Task GetReviewByIdAsync_WhenWorkflowServiceThrowsException_ShouldPropagateError()
    {
        var processId = Guid.NewGuid();

        var workflowClient = new WorkflowClientThrowingFake(new InvalidOperationException("Workflow service error"));
        var controller = new AdminReviewProcessesController(workflowClient);

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            controller.GetByIdAsync(processId, CancellationToken.None)
        );

        Assert.Equal("Workflow service error", exception.Message);
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

        public Guid ProcessId { get; private set; }

        public Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_listResponse);
        }

        public Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid processId, CancellationToken cancellationToken)
        {
            ProcessId = processId;
            return Task.FromResult(_singleResponse);
        }

        public Task ApproveReviewAsync(Guid processId, CancellationToken cancellationToken)
        {
            ProcessId = processId;
            return Task.CompletedTask;
        }

        public Task ReturnReviewAsync(Guid processId, ReturnRequestDto request, CancellationToken cancellationToken)
        {
            ProcessId = processId;
            return Task.CompletedTask;
        }
    }

    private sealed class WorkflowClientThrowingFake : IWorkflowClient
    {
        private readonly Exception _exceptionToThrow;

        public WorkflowClientThrowingFake(Exception exceptionToThrow)
        {
            _exceptionToThrow = exceptionToThrow;
        }

        public Task<IEnumerable<ReviewProcessListItemDto>> GetPendingReviewsAsync(CancellationToken cancellationToken) => throw _exceptionToThrow;

        public Task<ReviewProcessDetailDto?> GetReviewByIdAsync(Guid processId, CancellationToken cancellationToken) => throw _exceptionToThrow;

        public Task ApproveReviewAsync(Guid processId, CancellationToken cancellationToken) => throw _exceptionToThrow;

        public Task ReturnReviewAsync(Guid processId, ReturnRequestDto request, CancellationToken cancellationToken) => throw _exceptionToThrow;
    }
}