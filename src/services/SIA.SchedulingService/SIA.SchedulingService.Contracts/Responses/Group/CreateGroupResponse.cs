namespace SIA.SchedulingService.Contracts.Responses.Group;

public sealed record CreateGroupResponse
{
    public required Guid Id { get; init; }

    public required Guid TenantId { get; init; }

    public required Guid EducationalProgramId { get; init; }

    public required string GroupName { get; init; }

    public required string Shift { get; init; }

    public required int Capacity { get; init; }

    public required bool Status { get; init; }

    public required DateTime CreatedAtUtc { get; init; }

    public required Guid CorrelationId { get; init; }
}