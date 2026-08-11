using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class UpdateClassroomUseCase
{
    private readonly IClassroomDataStore _dataStore;

    public UpdateClassroomUseCase(IClassroomDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateClassroomResponse> ExecuteAsync(
        Guid tenantId,
        Guid classroomId,
        UpdateClassroomRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroom = await _dataStore.GetClassroomByIdAsync(tenantId, classroomId, cancellationToken);

        if (classroom is null)
        {
            throw new ClassroomNotFoundException(classroomId);
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (!string.Equals(classroom.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var codeExists = await _dataStore.ClassroomCodeExistsAsync(tenantId, normalizedCode, cancellationToken);
            if (codeExists)
            {
                throw new DuplicateClassroomCodeException(normalizedCode);
            }
        }

        classroom.Update(
            request.BuildingId,
            request.ClassroomTypeId,
            normalizedCode,
            request.Name,
            request.Capacity,
            request.Description);

        var integrationEvent = new ClassroomUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroom.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroom.TenantId,
            ClassroomId = classroom.Id,
            BuildingId = classroom.BuildingId,
            ClassroomTypeId = classroom.ClassroomTypeId,
            Code = classroom.Code,
            Name = classroom.Name,
            Capacity = classroom.Capacity,
            Status = classroom.Status,
            Version = 1
        };

        await _dataStore.UpdateClassroomWithOutboxAsync(classroom, integrationEvent, cancellationToken);

        return new UpdateClassroomResponse
        {
            Id = classroom.Id,
            TenantId = classroom.TenantId,
            BuildingId = classroom.BuildingId,
            ClassroomTypeId = classroom.ClassroomTypeId,
            Code = classroom.Code,
            Name = classroom.Name,
            Capacity = classroom.Capacity,
            Description = classroom.Description,
            Status = classroom.Status,
            UpdatedAtUtc = classroom.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}