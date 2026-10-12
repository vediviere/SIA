namespace SIA.AcademicStaffService.Contracts.Responses.Coordinators;

public sealed record CoordinatorResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid PersonId { get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required DateTime? UpdatedAtUtc { get; init; }
}