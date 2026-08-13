
namespace SIA.SchedulingService.Contracts.Requests.AcademicOffering;

public sealed record CreateAcademicOfferingRequest
{
    public required Guid TenantId { get; init; }
    public required Guid GroupId { get; init; }
    public required Guid SubjectId { get; init; }
    public required Guid AcademicLoadId { get; init; }
}