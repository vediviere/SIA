
namespace SIA.SchedulingService.Contracts.Responses.AcademicOffering;

public sealed record CreateAcademicOfferingResponse
{
    public required Guid Id { get; init; }
    public required Guid TenantId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid SubjectId { get; init; }
    public required Guid AcademicLoadId { get; init; }
    public required string OfferingStatus {  get; init; }
    public required bool Status { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public required Guid CorrelationId { get; init; }
}