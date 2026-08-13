namespace SIA.SchedulingService.Contracts.Requests;

public sealed record UpdateAcademicOfferingRequest
{
    public required Guid GroupId { get; init; }
    public required Guid SubjectId { get; init; }
    public required Guid AcademicLoadId { get; init; }
}