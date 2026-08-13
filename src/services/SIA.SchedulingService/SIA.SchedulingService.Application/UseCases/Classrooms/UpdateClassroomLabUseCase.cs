using SIA.SchedulingService.Application.Common.Exceptions.ClassroomLab;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;


namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class UpdateClassroomLabUseCase
{
    private readonly IClassroomLabDataStore _dataStore;

    public UpdateClassroomLabUseCase(IClassroomLabDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<UpdateClassroomLabResponse> ExecuteAsync(
        Guid tenantId,
        Guid classroomLabId,
        UpdateClassroomLabRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var classroomLab = await _dataStore.GetClassroomLabByIdAsync(tenantId, classroomLabId, cancellationToken);

        if (classroomLab is null)
        {
            throw new ClassroomLabNotFoundException(classroomLabId);
        }

        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        if (!string.Equals(classroomLab.Code, normalizedCode, StringComparison.OrdinalIgnoreCase))
        {
            var codeExists = await _dataStore.ClassroomLabCodeExistsAsync(tenantId, normalizedCode, cancellationToken);
            if (codeExists)
            {
                throw new DuplicateClassroomLabCodeException(normalizedCode);
            }
        }

        // Se eliminaron las asignaciones de BuildingId y ClassroomTypeId[cite: 7]
        classroomLab.Update(
            normalizedCode,
            request.Name,
            request.Capacity,
            request.Description);

        // Se eliminaron los IDs foráneos del evento para reflejar la inmutabilidad de la relación[cite: 7]
        var integrationEvent = new ClassroomLabUpdatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomLab.UpdatedAtUtc ?? DateTime.UtcNow,
            TenantId = classroomLab.TenantId,
            ClassroomLabId = classroomLab.Id,
            Code = classroomLab.Code,
            Name = classroomLab.Name,
            Capacity = classroomLab.Capacity,
            Status = classroomLab.Status,
            Version = 1
        };

        await _dataStore.UpdateClassroomLabWithOutboxAsync(classroomLab, integrationEvent, cancellationToken);

        return new UpdateClassroomLabResponse
        {
            Id = classroomLab.Id,
            TenantId = classroomLab.TenantId,
            BuildingId = classroomLab.BuildingId,
            ClassroomTypeId = classroomLab.ClassroomTypeId,
            Code = classroomLab.Code,
            Name = classroomLab.Name,
            Capacity = classroomLab.Capacity,
            Description = classroomLab.Description,
            Status = classroomLab.Status,
            UpdatedAtUtc = classroomLab.UpdatedAtUtc,
            CorrelationId = correlationId
        };
    }
}