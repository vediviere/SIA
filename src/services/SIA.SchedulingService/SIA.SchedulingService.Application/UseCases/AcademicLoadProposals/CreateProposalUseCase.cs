using ContractProposalStatus = SIA.SchedulingService.Contracts.Enums.ProposalStatus;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Requests.AcademicLoadProposal;
using SIA.SchedulingService.Contracts.Responses.AcademicLoadProposal;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoadProposals;

public sealed class CreateProposalUseCase
{
  private readonly IProposalDataStore _dataStore;

  public CreateProposalUseCase(IProposalDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task<CreateProposalResponse> ExecuteAsync(CreateProposalRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var exists = await _dataStore.ExistsAsync(request.TenantId, request.EducationalProgramId, request.AcademicPeriodId, cancellationToken);

    if (exists)
    {
      throw new ProposalAlreadyExistsException(request.EducationalProgramId, request.AcademicPeriodId);
    }

    var proposal = new Proposal(request.TenantId, request.EducationalProgramId, request.AcademicPeriodId, request.DivisionHeadId);
    var proposalStatus = (ContractProposalStatus)proposal.ProposalStatus;

    var integrationEvent = new ProposalCreatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = proposal.CreatedAtUtc,
      TenantId = proposal.TenantId,
      ProposalId = proposal.Id,
      EducationalProgramId = proposal.EducationalProgramId,
      AcademicPeriodId = proposal.AcademicPeriodId,
      DivisionHeadId = proposal.DivisionHeadId,
      ProposalStatus = proposalStatus,
      Status = proposal.Status,
      Version = 1
    };

    await _dataStore.AddWithOutboxAsync(proposal, integrationEvent, cancellationToken);

    return new CreateProposalResponse
    {
      Id = proposal.Id,
      TenantId = proposal.TenantId,
      EducationalProgramId = proposal.EducationalProgramId,
      AcademicPeriodId = proposal.AcademicPeriodId,
      DivisionHeadId = proposal.DivisionHeadId,
      ProposalStatus = proposalStatus,
      Status = proposal.Status,
      CreatedAtUtc = proposal.CreatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
