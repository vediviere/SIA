using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Contracts.Responses;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class UpdateAcademicOfferingUseCase
{
  private readonly IAcademicOfferingDataStore _dataStore;
  private readonly IAcademicLoadDataStore _academicLoadDataStore;
  private readonly AcademicLoadClassHoursCalculator _classHoursCalculator;
  private readonly ProposalValidator _proposalValidator;

  public UpdateAcademicOfferingUseCase(IAcademicOfferingDataStore dataStore, IAcademicLoadDataStore academicLoadDataStore, AcademicLoadClassHoursCalculator classHoursCalculator, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _academicLoadDataStore = academicLoadDataStore;
    _classHoursCalculator = classHoursCalculator;
    _proposalValidator = proposalValidator;
  }

  public async Task<UpdateAcademicOfferingResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateAcademicOfferingRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicOffering = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

    if (academicOffering is null)
    {
      throw new AcademicOfferingNotFoundException(id);
    }

    var academicLoad = await _academicLoadDataStore.GetByIdAsync(tenantId, academicOffering.AcademicLoadId, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(academicOffering.AcademicLoadId);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    academicOffering.Update(request.OfferingStatus);
    await _classHoursCalculator.RecalculateAsync(academicLoad, academicOffering, cancellationToken);

    var integrationEvent = new AcademicOfferingUpdatedIntegrationEvent
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicOffering.UpdatedAtUtc!.Value,
      TenantId = academicOffering.TenantId,
      OfferingId = academicOffering.Id,
      OfferingStatus = academicOffering.OfferingStatus,
      ClassHours = academicOffering.ClassHours,
      Status = academicOffering.Status,
      Version = 1
    };

    await _dataStore.UpdateAcademicOfferingWithOutboxAsync(academicOffering, academicLoad, integrationEvent, cancellationToken);

    return new UpdateAcademicOfferingResponse
    {
      Id = academicOffering.Id,
      TenantId = academicOffering.TenantId,
      GroupId = academicOffering.GroupId,
      SubjectId = academicOffering.SubjectId,
      AcademicLoadId = academicOffering.AcademicLoadId,
      OfferingStatus = academicOffering.OfferingStatus,
      ClassHours = academicOffering.ClassHours,
      Status = academicOffering.Status,
      CreatedAtUtc = academicOffering.CreatedAtUtc,
      UpdatedAtUtc = academicOffering.UpdatedAtUtc,
      CorrelationId = correlationId
    };
  }
}
