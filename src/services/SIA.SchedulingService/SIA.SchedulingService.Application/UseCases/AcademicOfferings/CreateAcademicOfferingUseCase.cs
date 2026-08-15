
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.AcademicOffering;
using SIA.SchedulingService.Contracts.Requests.AcademicOffering;
using SIA.SchedulingService.Contracts.Responses.AcademicOffering;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class CreateAcademicOfferingUseCase
{
    private readonly IAcademicOfferingDataStore _dataStore;

    public CreateAcademicOfferingUseCase(IAcademicOfferingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateAcademicOfferingResponse> ExecuteAsync(CreateAcademicOfferingRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var offeringExists = await _dataStore.ExistsByGroupAndSubjectAsync(request.GroupId, request.SubjectId, cancellationToken);

        if (offeringExists)
        {
            throw new AcademicOfferingAlreadyExistsException(request.GroupId, request.SubjectId);
        }

        var academicOffering = new AcademicOffering(
            request.TenantId,
            request.GroupId,
            request.SubjectId,
            request.AcademicLoadId,
            request.OfferingStatus);

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
            Status = academicOffering.Status,
            Version = 1
        };

        await  _dataStore.AddAcademicOfferingWithOutboxAsync(academicOffering, integrationEvent, cancellationToken);

        return new CreateAcademicOfferingResponse
        {
            Id = academicOffering.Id,
            TenantId = academicOffering.TenantId,
            GroupId = academicOffering.GroupId,
            SubjectId = academicOffering.SubjectId,
            AcademicLoadId = academicOffering.AcademicLoadId,
            OfferingStatus = academicOffering.OfferingStatus,
            Status = academicOffering.Status,
            CreatedAtUtc = academicOffering.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }

}