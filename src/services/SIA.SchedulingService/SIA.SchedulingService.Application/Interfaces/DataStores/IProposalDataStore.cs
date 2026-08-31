using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.Interfaces.DataStores;

public interface IProposalDataStore
{
  Task<Proposal?> GetByIdAsync(Guid tenantId, Guid proposalId, CancellationToken cancellationToken);
  Task<bool> ExistsAsync(Guid tenantId, Guid educationalProgramId, Guid academicPeriodId, CancellationToken cancellationToken);
  Task AddWithOutboxAsync(Proposal proposal, ProposalCreatedIntegrationEvent integrationEvent, CancellationToken cancellationToken);
}
