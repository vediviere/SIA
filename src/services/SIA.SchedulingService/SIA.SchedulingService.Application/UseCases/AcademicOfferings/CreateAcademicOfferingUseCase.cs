using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Common.Services.AcademicLoads;
using SIA.SchedulingService.Application.Common.Services.AcademicLoadProposals;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Contracts.Responses.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class CreateAcademicOfferingUseCase
{
  private readonly IAcademicOfferingDataStore _dataStore;
  private readonly IAcademicLoadDataStore _academicLoadDataStore;
  private readonly AcademicLoadClassHoursCalculator _classHoursCalculator;
  private readonly ProposalValidator _proposalValidator;

  public CreateAcademicOfferingUseCase(IAcademicOfferingDataStore dataStore, IAcademicLoadDataStore academicLoadDataStore, AcademicLoadClassHoursCalculator classHoursCalculator, ProposalValidator proposalValidator)
  {
    _dataStore = dataStore;
    _academicLoadDataStore = academicLoadDataStore;
    _classHoursCalculator = classHoursCalculator;
    _proposalValidator = proposalValidator;
  }

  public async Task<CreateAcademicOfferingResponse> ExecuteAsync(CreateAcademicOfferingRequest request, Guid correlationId, CancellationToken cancellationToken)
  {
    var academicLoad = await _academicLoadDataStore.GetByIdAsync(request.TenantId, request.AcademicLoadId, cancellationToken);

    if (academicLoad is null)
    {
      throw new AcademicLoadNotFoundException(request.AcademicLoadId);
    }

    await _proposalValidator.EnsureEditableAsync(academicLoad, cancellationToken);

    var offeringExists = await _dataStore.ExistsByGroupAndSubjectAsync(request.TenantId, request.GroupId, request.SubjectId, cancellationToken);

    if (offeringExists)
    {
      throw new AcademicOfferingAlreadyExistsException(request.GroupId, request.SubjectId);
    }

    var academicOffering = new AcademicOffering(request.TenantId, request.GroupId, request.SubjectId, request.AcademicLoadId, request.OfferingStatus);

    await _classHoursCalculator.RecalculateAsync(academicLoad, academicOffering, cancellationToken);

    var integrationEvent = new AcademicOfferingCreatedIntegrationEvet
    {
      EventId = Guid.NewGuid(),
      CorrelationId = correlationId,
      OccurredAtUtc = academicOffering.CreatedAtUtc,
      TenantId = academicOffering.TenantId,
      OfferingId = academicOffering.Id,
      GroupId = academicOffering.GroupId,
      SubjectId = academicOffering.SubjectId,
      AcademicLoadId = academicOffering.AcademicLoadId,
      OfferingStatus = academicOffering.OfferingStatus,
      ClassHours = academicOffering.ClassHours,
      Status = academicOffering.Status,
      Version = 1
    };

    await _dataStore.AddAcademicOfferingWithOutboxAsync(academicOffering, academicLoad, integrationEvent, cancellationToken);

    return new CreateAcademicOfferingResponse
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
      CorrelationId = correlationId
    };
  }
}
