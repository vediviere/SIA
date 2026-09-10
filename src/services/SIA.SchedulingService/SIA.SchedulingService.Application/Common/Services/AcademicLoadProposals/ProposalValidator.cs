using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicLoadProposal;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Domain.Enums;

namespace SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;

public sealed class ProposalValidator
{
  private readonly IProposalDataStore _dataStore;

  public ProposalValidator(IProposalDataStore dataStore)
  {
    _dataStore = dataStore;
  }

  public async Task EnsureEditableAsync(AcademicLoad academicLoad, CancellationToken cancellationToken)
  {
    if (!academicLoad.Status)
    {
      throw new AcademicLoadNotEditableException(academicLoad.Id);
    }

    await EnsureEditableAsync(academicLoad.TenantId, academicLoad.ProposalId, academicLoad.AcademicPeriodId, cancellationToken);
  }

  public async Task EnsureEditableAsync(Guid tenantId, Guid proposalId, Guid academicPeriodId, CancellationToken cancellationToken)
  {
    var proposal = await _dataStore.GetByIdAsync(tenantId, proposalId, cancellationToken);

    if (proposal is null ||
        !proposal.Status ||
        proposal.ProposalStatus != ProposalStatus.Draft ||
        proposal.AcademicPeriodId != academicPeriodId)
    {
      throw new ProposalNotEditableException(proposalId);
    }
  }
}
