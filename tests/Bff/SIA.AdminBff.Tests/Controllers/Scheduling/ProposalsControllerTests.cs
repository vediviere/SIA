using Microsoft.AspNetCore.Mvc;
using SIA.AdminBff.Clients.Scheduling;
using SIA.AdminBff.Contracts.Scheduling.Enums;
using SIA.AdminBff.Contracts.Scheduling.Requests;
using SIA.AdminBff.Contracts.Scheduling.Responses;
using SIA.AdminBff.Controllers.Scheduling;
using SIA.AdminBff.Infrastructure.Tenancy;

namespace SIA.AdminBff.Tests.Controllers.Scheduling;

public sealed class ProposalsControllerTests
{
    [Fact]
    public async Task CreateAsync_WithValidRequest_ShouldCreateProposal()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var programId = Guid.NewGuid();
        var periodId = Guid.NewGuid();
        var divisionHeadId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var client = new SchedulingClientFake(new ProposalDto
        {
            Id = proposalId,
            TenantId = tenantId,
            EducationalProgramId = programId,
            AcademicPeriodId = periodId,
            DivisionHeadId = divisionHeadId,
            ProposalStatus = ProposalStatus.Draft,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        });

        var controller = new ProposalsController(client, new TenantContextFake(tenantId));

        // Act
        var result = await controller.CreateAsync(new CreateProposalRequest
        {
            EducationalProgramId = programId,
            AcademicPeriodId = periodId,
            DivisionHeadId = divisionHeadId
        }, CancellationToken.None);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result.Result);
        var response = Assert.IsType<ProposalResponse>(objectResult.Value);

        Assert.Equal(201, objectResult.StatusCode);
        Assert.Equal(proposalId, response.Id);
        Assert.Equal(ProposalStatus.Draft, response.ProposalStatus);
        Assert.Equal(programId, client.ProgramId);
        Assert.Equal(periodId, client.PeriodId);
        Assert.Equal(divisionHeadId, client.DivisionHeadId);
    }

    [Fact]
    public async Task SubmitForReviewAsync_WithValidProposal_ShouldReturnSubmittedStatus()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var proposalId = Guid.NewGuid();

        var client = new SchedulingClientFake(new ProposalDto
        {
            Id = proposalId,
            TenantId = tenantId,
            EducationalProgramId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            DivisionHeadId = Guid.NewGuid(),
            ProposalStatus = ProposalStatus.SubmittedForReview,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            UpdatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        });

        var controller = new ProposalsController(client, new TenantContextFake(tenantId));

        // Act
        var result = await controller.SubmitForReviewAsync(proposalId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var response = Assert.IsType<ProposalResponse>(okResult.Value);

        Assert.Equal(proposalId, response.Id);
        Assert.Equal(ProposalStatus.SubmittedForReview, response.ProposalStatus);
        Assert.NotNull(response.UpdatedAtUtc);
        Assert.Equal(proposalId, client.ProposalId);
    }

    [Fact]
    public void FromService_WithRequiresCorrection_ShouldPreserveStatus()
    {
        // Arrange
        var serviceResponse = new ProposalDto
        {
            Id = Guid.NewGuid(),
            TenantId = Guid.NewGuid(),
            EducationalProgramId = Guid.NewGuid(),
            AcademicPeriodId = Guid.NewGuid(),
            DivisionHeadId = Guid.NewGuid(),
            ProposalStatus = ProposalStatus.RequiresCorrection,
            Status = true,
            CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
            UpdatedAtUtc = DateTime.UtcNow,
            CorrelationId = Guid.NewGuid()
        };

        // Act
        var response = ProposalResponse.FromService(serviceResponse);

        // Assert
        Assert.Equal(ProposalStatus.RequiresCorrection, response.ProposalStatus);
        Assert.Equal(serviceResponse.Id, response.Id);
        Assert.Equal(serviceResponse.CorrelationId, response.CorrelationId);
    }

    private sealed class TenantContextFake : ITenantContext
    {
        public TenantContextFake(Guid tenantId)
        {
            TenantId = tenantId;
        }

        public Guid TenantId { get; }
    }

    private sealed class SchedulingClientFake : ISchedulingClient
    {
        private readonly ProposalDto _response;

        public SchedulingClientFake(ProposalDto response)
        {
            _response = response;
        }

        public Guid ProgramId { get; private set; }
        public Guid PeriodId { get; private set; }
        public Guid DivisionHeadId { get; private set; }
        public Guid ProposalId { get; private set; }

        public Task<ProposalDto> CreateProposalAsync(Guid educationalProgramId, Guid academicPeriodId, Guid divisionHeadId, CancellationToken cancellationToken)
        {
            ProgramId = educationalProgramId;
            PeriodId = academicPeriodId;
            DivisionHeadId = divisionHeadId;

            return Task.FromResult(_response);
        }

        public Task<ProposalDto> SubmitForReviewAsync(Guid proposalId, CancellationToken cancellationToken)
        {
            ProposalId = proposalId;

            return Task.FromResult(_response);
        }

        public Task<IReadOnlyCollection<TeacherCandidateDto>> GetTeacherCandidatesAsync(Guid educationalProgramId, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<LoadDto> CreateLoadAsync(LoadCreateDto request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<LoadDto> UpdateLoadAsync(Guid loadId, LoadUpdateDto request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<SupportHourDto> CreateSupportHourAsync(SupportHourCreateDto request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public Task<SupportHourDto> UpdateSupportHourAsync(Guid supportHourId, SupportHourUpdateDto request, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }
    }
}