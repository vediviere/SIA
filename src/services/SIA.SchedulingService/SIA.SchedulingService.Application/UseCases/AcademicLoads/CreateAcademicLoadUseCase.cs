using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicLoad;
using SIA.SchedulingService.Contracts.Requests.AcademicLoad;
using SIA.SchedulingService.Contracts.Responses.AcademicLoad;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;

namespace SIA.SchedulingService.Application.UseCases.AcademicLoads;

public sealed class CreateAcademicLoadUseCase
{
  private readonly IAcademicLoadDataStore _dataStore;
  private readonly ProposalValidator _proposalValidator;

  public CreateAcademicLoadUseCase(IAcademicLoadDataStore dataStore, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _proposalValidator = proposalValidator;
  }

  public async Task<CreateAcademicLoadResponse> ExecuteAsync(CreateAcademicLoadRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    await _proposalValidator.EnsureEditableAsync(request.TenantId, request.ProposalId, request.AcademicPeriodId, cancellationToken);

    var academicLoad = new AcademicLoad(
        request.TenantId,
        request.ProposalId,
        request.TeacherId,
        request.DivisionId,
        request.AcademicPeriodId,
        request.OfficialLetterNumber,
        request.ProposedDate,
        0,
        0,
        request.AssignmentDate);

    var integrationEvent = new AcademicLoadCreatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicLoad.CreatedAtUtc,
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

    await _dataStore.AddAcademicLoadWithOutboxAsync(academicLoad, integrationEvent, cancellationToken);

    return new CreateAcademicLoadResponse
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
      CorrelationId = correlationId
    };
  }

}
