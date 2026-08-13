namespace SIA.AcademicStaffService.Contracts.Requests.Coordinators;

public sealed record CreateCoordinatorRequest
{
    public required Guid TenantId { get; init; }

    public required Guid PersonId { get; init; }

    public required string AcademicDegree { get; init; }
}