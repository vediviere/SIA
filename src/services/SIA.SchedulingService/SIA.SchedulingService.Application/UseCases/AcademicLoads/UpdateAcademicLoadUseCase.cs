using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoads;

public sealed class UpdateAcademicLoadUseCase
{
  private readonly IAcademicLoadDataStore _dataStore;
  private readonly ProposalValidator _proposalValidator;

  public UpdateAcademicLoadUseCase(IAcademicLoadDataStore dataStore, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _proposalValidator = proposalValidator;
  }

  public async Task<UpdateAcademicLoadResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateAcademicLoadRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicLoad = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

    if (academicLoad == null)
    {
      throw new AcademicLoadNotFoundException(id);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    academicLoad.Update(request.OfficialLetterNumber, request.ProposedDate, request.AssignmentDate);

    var integrationEvent = new AcademicLoadUpdatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicLoad.UpdatedAtUtc!.Value,
      TenantId = academicLoad.TenantId,
      ProposalId = academicLoad.ProposalId,
      AcademicLoadId = academicLoad.Id,
      TeacherId = academicLoad.TeacherId,
      DivisionId = academicLoad.DivisionId,
      AcademicPeriodId = academicLoad.AcademicPeriodId,
      OfficialLetterNumber = academicLoad.OfficialLetterNumber,
      ProposedDate = academicLoad.ProposedDate,
      AssignmentDate = academicLoad.AssignmentDate,
      ClassHours = academicLoad.ClassHours,
      SupportHours = academicLoad.SupportHours,
      Status = academicLoad.Status,
      Version = 1
    };

    await _dataStore.UpdateAcademicLoadWithOutboxAsync(academicLoad, integrationEvent, cancellationToken);

    return new UpdateAcademicLoadResponse
    {
      Id = academicLoad.Id,
      TenantId = academicLoad.TenantId,
      ProposalId = academicLoad.ProposalId,
      TeacherId = academicLoad.TeacherId,
      DivisionId = academicLoad.DivisionId,
      AcademicPeriodId = academicLoad.AcademicPeriodId,
      OfficialLetterNumber = academicLoad.OfficialLetterNumber,
      ProposedDate = academicLoad.ProposedDate,
      ClassHours = academicLoad.ClassHours,
      SupportHours = academicLoad.SupportHours,
      AssignmentDate = academicLoad.AssignmentDate,
      Status = academicLoad.Status,
      CreatedAtUtc = academicLoad.CreatedAtUtc,
      UpdatedAtUtc = academicLoad.UpdatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
