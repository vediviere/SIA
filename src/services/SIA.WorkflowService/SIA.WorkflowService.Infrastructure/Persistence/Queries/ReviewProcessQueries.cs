using Microsoft.EntityFrameworkCore;
using SIA.WorkflowService.Application.Interfaces.Queries;
using SIA.WorkflowService.Contracts.Enums;
using SIA.WorkflowService.Contracts.Responses.ReviewProcesses;
using SIA.WorkflowService.Infrastructure.Persistence.Contexts;
using DomainReviewStatus = SIA.WorkflowService.Domain.Enums.ReviewStatus;

namespace SIA.WorkflowService.Infrastructure.Persistence.Queries;

public sealed class ReviewProcessQueries : IReviewProcessQueries
{
    private readonly WorkflowDbContext _dbContext;

    public ReviewProcessQueries(WorkflowDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<ReviewProcessListItemResponse>> GetInReviewAsync(
        Guid tenantId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ReviewProcesses
            .AsNoTracking()
            .Where(process => process.TenantId == tenantId &&
                              process.Status == DomainReviewStatus.InReview)
            .OrderBy(process => process.SubmittedAtUtc)
            .Select(process => new ReviewProcessListItemResponse
            {
                Id = process.Id,
                AcademicLoadProposalId = process.AcademicLoadProposalId,
                DivisionHeadId = process.DivisionHeadId,
                Version = process.Version,
                SubmittedAtUtc = process.SubmittedAtUtc,
                CreatedAtUtc = process.CreatedAtUtc
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<ReviewProcessResponse?> GetByIdAsync(
        Guid tenantId,
        Guid processId,
        CancellationToken cancellationToken)
    {
        return await _dbContext.ReviewProcesses
            .AsNoTracking()
            .Where(process => process.TenantId == tenantId &&
                              process.Id == processId)
            .Select(process => new ReviewProcessResponse
            {
                Id = process.Id,
                AcademicLoadProposalId = process.AcademicLoadProposalId,
                DivisionHeadId = process.DivisionHeadId,
                Version = process.Version,
                Status = (int)process.Status,
                SubmittedAtUtc = process.SubmittedAtUtc,
                CreatedAtUtc = process.CreatedAtUtc,
                UpdatedAtUtc = process.UpdatedAtUtc,
                CorrelationId = process.CorrelationId,

                Observations = process.Observations
                    .OrderBy(observation => observation.CreatedAtUtc)
                    .Select(observation => new ReviewObservationResponse
                    {
                        Id = observation.Id,
                        TargetType = (ObservationTarget)observation.TargetType,
                        TargetId = observation.TargetId,
                        Description = observation.Description,
                        CreatedBy = observation.CreatedBy,
                        CreatedAtUtc = observation.CreatedAtUtc
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}