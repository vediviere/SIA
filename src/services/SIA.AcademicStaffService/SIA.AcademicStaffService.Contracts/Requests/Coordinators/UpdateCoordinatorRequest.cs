namespace SIA.AcademicStaffService.Contracts.Requests.Coordinators;

public sealed record UpdateCoordinatorRequest
{
    public required string AcademicDegree { get; init; }
}