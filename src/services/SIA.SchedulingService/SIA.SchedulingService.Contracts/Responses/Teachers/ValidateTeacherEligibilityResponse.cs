using SIA.SchedulingService.Contracts.Enums;

namespace SIA.SchedulingService.Contracts.Responses.Teachers;

public sealed record ValidateTeacherEligibilityResponse
{
    public required bool Eligible { get; init; }
    public required IReadOnlyCollection<TeacherEligibilityFailureReason> Reasons { get; init; }
}