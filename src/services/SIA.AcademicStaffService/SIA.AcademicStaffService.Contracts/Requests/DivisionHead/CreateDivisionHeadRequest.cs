namespace SIA.AcademicStaffService.Contracts.Requests.DivisionManagers;

public sealed record CreateDivisionHeadRequest
{

    public required Guid ProgramId { get; init; }

    public required Guid PersonId { get; init; }

}