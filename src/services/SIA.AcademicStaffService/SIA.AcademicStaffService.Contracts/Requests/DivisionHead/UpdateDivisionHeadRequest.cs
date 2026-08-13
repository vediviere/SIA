namespace SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;

public sealed record UpdateDivisionHeadRequest
{
    public required string AcademicDegree { get; init; }
}