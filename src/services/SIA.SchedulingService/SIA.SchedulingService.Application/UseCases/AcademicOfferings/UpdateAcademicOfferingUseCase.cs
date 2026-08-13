using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Common.Exceptions.AcademicOffering;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents;
using SIA.SchedulingService.Contracts.Requests;
using SIA.SchedulingService.Contracts.Responses;

namespace SIA.SchedulingService.Application.UseCases.AcademicOfferings;

public sealed class UpdateAcademicOfferingUseCase
{
    private readonly IAcademicOfferingDataStore _dataStore;

    public UpdateAcademicOfferingUseCase(IAcademicOfferingDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateAcademicOfferingResponse> ExecuteAsync(Guid tenantId, Guid id, UpdateAcademicOfferingRequest request, Guid correlationId, CancellationToken cancellationToken)
    {
        var academicOffering = await _dataStore.GetByIdAsync(tenantId, id, cancellationToken);

        if (academicOffering is null)
        {
            throw new AcademicOfferingNotFoundException(id);
        }

        if (request.GroupId != academicOffering.GroupId || request.SubjectId != academicOffering.SubjectId)
        {
            var offeringExists = await _dataStore.ExistsByGroupAndSubjectAsync(request.GroupId, request.SubjectId, cancellationToken);

            if (offeringExists)
            {
                throw new AcademicOfferingAlreadyExistsException(request.GroupId, request.SubjectId);
            }
        }

        academicOffering.Update(request.GroupId, request.SubjectId, request.AcademicLoadId);

        var integrationEvent = new AcademicOfferingUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = academicOffering.UpdatedAtUtc!.Value,
            TenantId = academicOffering.TenantId,
            OfferingId = academicOffering.Id,
            GroupId = academicOffering.GroupId,
            SubjectId = academicOffering.SubjectId,
            AcademicLoadId = academicOffering.AcademicLoadId,
            Status = academicOffering.Status,
            Version = 1
        };

        await _dataStore.UpdateAcademicOfferingWithOutboxAsync(academicOffering, integrationEvent, cancellationToken);

        return new UpdateAcademicOfferingResponse
        {
            Id = academicOffering.Id,
            TenantId = academicOffering.TenantId,
            GroupId = academicOffering.GroupId,
            SubjectId = academicOffering.SubjectId,
            AcademicLoadId = academicOffering.AcademicLoadId,
            Status = academicOffering.Status,
            CreatedAtUtc = academicOffering.CreatedAtUtc,
            UpdatedAtUtc = academicOffering.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}