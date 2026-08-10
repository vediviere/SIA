using SIA.SchedulingService.Application.Common.Exceptions;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.Requests.Classroom;
using SIA.SchedulingService.Contracts.Responses.Classrooms;
using SIA.SchedulingService.Domain.Entities;

namespace SIA.SchedulingService.Application.UseCases.Classrooms;

public sealed class CreateClassroomUseCase
{
    private readonly IClassroomDataStore _dataStore;

    public CreateClassroomUseCase(IClassroomDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateClassroomResponse> ExecuteAsync(
        CreateClassroomRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var normalizedCode = request.Code.Trim().ToUpperInvariant();

        var codeExists = await _dataStore.ClassroomCodeExistsAsync(
            request.TenantId,
            normalizedCode,
            cancellationToken);

        if (codeExists)
        {
            throw new DuplicateClassroomCodeException(normalizedCode);
        }

        var classroom = new Classroom(
            request.TenantId,
            request.BuildingId,
            request.ClassroomTypeId,
            normalizedCode,
            request.Name,
            request.Capacity,
            request.Description);

        var integrationEvent = new ClassroomCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroom.CreatedAtUtc,
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

        await _dataStore.AddClassroomWithOutboxAsync(classroom, integrationEvent, cancellationToken);

        return new CreateClassroomResponse
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
            CreatedAtUtc = classroom.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}
