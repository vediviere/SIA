using SIA.SchedulingService.Application.Common.Exceptions.ClassroomType;
using SIA.SchedulingService.Application.Interfaces.DataStores;
using SIA.SchedulingService.Contracts.IntegrationEvents.ClassroomTypes;
using SIA.SchedulingService.Contracts.Requests.ClassroomType;
using SIA.SchedulingService.Contracts.Responses.ClassroomType;
using SIA.SchedulingService.Domain.Entities;


namespace SIA.SchedulingService.Application.UseCases.ClassroomTypes;

public sealed class CreateClassroomTypeUseCase
{
    private readonly IClassroomTypeDataStore _dataStore;

    public CreateClassroomTypeUseCase(IClassroomTypeDataStore dataStore)
    {
        _dataStore = dataStore;
    }

    public async Task<CreateClassroomTypeResponse> ExecuteAsync(
        CreateClassroomTypeRequest request,
        Guid correlationId,
        CancellationToken cancellationToken)
    {
        var normalizedName = request.Name.Trim();

        var nameExists = await _dataStore.ClassroomTypeNameExistsAsync(
            request.TenantId,
            normalizedName,
            cancellationToken);

        if (nameExists)
        {
            throw new DuplicateClassroomTypeNameException(normalizedName);
        }

        var classroomType = new ClassroomType(
            request.TenantId,
            request.Code,
            normalizedName,
            request.Description);

        var integrationEvent = new ClassroomTypeCreatedIntegrationEvent
        {
            EventId = Guid.NewGuid(),
            CorrelationId = correlationId,
            OccurredAtUtc = classroomType.CreatedAtUtc,
            TenantId = classroomType.TenantId,
            ClassroomTypeId = classroomType.Id,
            Code = classroomType.Code,
            Name = classroomType.Name,
            Description = classroomType.Description,
            Status = classroomType.Status,
            Version = 1
        };

        await _dataStore.AddClassroomTypeWithOutboxAsync(classroomType, integrationEvent, cancellationToken);

        return new CreateClassroomTypeResponse
        {
            Id = classroomType.Id,
            TenantId = classroomType.TenantId,
            Code = classroomType.Code,
            Name = classroomType.Name,
            Description = classroomType.Description,
            Status = classroomType.Status,
            CreatedAtUtc = classroomType.CreatedAtUtc,
            CorrelationId = correlationId
        };
    }
}