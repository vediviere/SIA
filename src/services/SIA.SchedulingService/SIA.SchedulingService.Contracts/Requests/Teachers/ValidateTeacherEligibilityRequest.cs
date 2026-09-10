namespace SIA.SchedulingService.Contracts.Requests.Teachers;

public sealed record ValidateTeacherEligibilityRequest
{
    public required Guid TenantId { get; init; }
    public required Guid AcademicPeriodId { get; init; }
    public required Guid TeacherId { get; init; }
    public required Guid AcademicOfferingId { get; init; }
    public required Guid GroupId { get; init; }
}