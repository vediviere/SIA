using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;
using SIA.SchedulingService.Domain.Entities;
using SIA.SchedulingService.Contracts.IntegrationEvents.Classrooms;

namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class CreateClassroomLabUseCase
{
    private readonly IClassroomLabDataStore _dataStore;

    public CreateClassroomLabUseCase(IClassroomLabDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateClassroomLabResponse> ExecuteAsync(
        CreateClassroomLabRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _dataStore.ClassroomLabCodeExistsAsync(
            request.TenantId,
            normalizedCode,
            cancellationToken);

        if (codeExists)
        {
            throw new DuplicateClassroomLabCodeException(normalizedCode);
        }

        var classroomLab = new ClassroomLab(
            request.TenantId,
            request.BuildingId,
            request.ClassroomTypeId,
            normalizedCode,
            request.Name,
            request.Capacity,
            request.Description);

        var integrationEvent = new ClassroomLabCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomLab.CreatedAtUtc,
            TenantId = classroomLab.TenantId,
            ClassroomLabId = classroomLab.Id,
            BuildingId = classroomLab.BuildingId,
            ClassroomTypeId = classroomLab.ClassroomTypeId,
            Code = classroomLab.Code,
            Name = classroomLab.Name,
            Capacity = classroomLab.Capacity,
            Status = classroomLab.Status,
            Version = 1
        };

        await _dataStore.AddClassroomLabWithOutboxAsync(classroomLab, integrationEvent, cancellationToken);

        return new CreateClassroomLabResponse
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
            CreatedAtUtc = classroomLab.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}