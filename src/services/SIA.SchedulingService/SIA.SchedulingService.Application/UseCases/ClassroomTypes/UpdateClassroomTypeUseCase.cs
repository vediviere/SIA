using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.ClassroomTypes;

public sealed class UpdateClassroomTypeUseCase
{
    private readonly IClassroomTypeDataStore _dataStore;

    public UpdateClassroomTypeUseCase(IClassroomTypeDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateClassroomTypeResponse> ExecuteAsync(
        Guid tenantId,
        Guid classroomTypeId,
        UpdateClassroomTypeRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroomType = await _dataStore.GetClassroomTypeByIdAsync(tenantId, classroomTypeId, cancellationToken);

        if (classroomType is null)
        {
            throw new ClassroomTypeNotFoundException(classroomTypeId);
        }

        var normalizedName = request.Name.Trim();

        if (!string.Equals(classroomType.Name, normalizedName, StringComparison.OrdinalIgnoreCase))
        {
            var nameExists = await _dataStore.ClassroomTypeNameExistsAsync(tenantId, normalizedName, cancellationToken);
            if (nameExists)
            {
                throw new DuplicateClassroomTypeNameException(normalizedName);
            }
        }

        classroomType.Update(request.Code, normalizedName, request.Description);

        var integrationEvent = new ClassroomTypeUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomType.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroomType.TenantId,
            ClassroomTypeId = classroomType.Id,
            Code = classroomType.Code,
            Name = classroomType.Name,
            Description = classroomType.Description,
            Status = classroomType.Status,
            Version = 1
        };

        await _dataStore.UpdateClassroomTypeWithOutboxAsync(classroomType, integrationEvent, cancellationToken);

        return new UpdateClassroomTypeResponse
        {
            Id = classroomType.Id,
            TenantId = classroomType.TenantId,
            Code = classroomType.Code,
            Name = classroomType.Name,
            Description = classroomType.Description,
            Status = classroomType.Status,
            UpdatedAtUtc = classroomType.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}